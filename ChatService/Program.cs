using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatService.Configuration;
using ChatService.Data;
using ChatService.Extensions;
using ChatService.Hubs;
using ChatService.Interfaces;
using ChatService.Mapper;
using ChatService.Models;
using ChatService.Providers;
using ChatService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;



var builder = WebApplication.CreateBuilder(args);

var enviroment = builder.Environment;
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

if (enviroment.IsProduction())
{
    builder.Configuration.LoadVaultSecrets(builder.Configuration, logger);

    builder.Configuration["CORS:AllowedOrigins"] = "https://labchat.phlab.software";
}

builder.Services.AddDbContext<ChatServiceDbContext>(opts =>
{
    logger.LogInformation(builder.Configuration["ConnectionStrings:DefaultConnection"]);
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});


builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("Minio"));


builder.Services.AddScoped<IMapperService, MapperService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<IS3Service, MinioService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService.Services.ChatService>();
builder.Services.AddSingleton<ISharedMemoryConnectionDB, SharedMemoryConnectionsDB>();

builder.Services.AddSignalR().AddHubOptions<ChatHub>(o => { o.ClientTimeoutInterval = TimeSpan.FromSeconds(60); });
builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IUserIdProvider), typeof(UserIdProvider)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("chat", policyBuilder =>
    {
        policyBuilder.WithOrigins(builder.Configuration["CORS:AllowedOrigins"]);
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowCredentials();
    });
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 4;
    })
    .AddEntityFrameworkStores<ChatServiceDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    x.JsonSerializerOptions.WriteIndented = true;
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
            if (!string.IsNullOrEmpty(accessToken))
                ctx.Token = accessToken;

            return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
            ctx.Response.StatusCode = 401;
            ctx.HandleResponse();
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ChatServiceDbContext>();
        var pendingMigrations = context.Database.GetPendingMigrations().Any();
        if (!context.Database.GetAppliedMigrations().Any() && !pendingMigrations)
        {
            // Crie uma migração inicial se não houver migrações
            context.Database.ExecuteSqlRaw("CREATE TABLE __EFMigrationsHistory (MigrationId nvarchar(150) NOT NULL, ProductVersion nvarchar(32) NOT NULL);");
            // Adicione a migração inicial
            context.Database.Migrate();
        }
        if (pendingMigrations)
        {
            context.Database.Migrate();
        }
    }
    catch (Exception e)
    {
        logger.LogError(e, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("chat");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/connectchat");

app.Run();