using System.Text;
using ChatService.Configuration;
using ChatService.Data;
using ChatService.Hubs;
using ChatService.Interfaces;
using ChatService.Mapper;
using ChatService.Models;
using ChatService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChatServiceTests.Fixtures;

public class TestStartup
{
    public TestStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add configuration
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Jwt:Key", "CHAVESUPERSECRETAPARATESTESQUETEMQUEBATER256BITS" },
                { "Minio:Endpoint", "s3.teste.lab" },
                { "Minio:AccessKey", "testAccessKey" },
                { "Minio:SecretKey", "testSecretKey" },
                { "Minio:BucketName", "testBucketName" }
            });

        var configuration = configBuilder.Build();
        services.AddSingleton<IConfiguration>(configuration);


        // Add DbContext
        services.AddDbContext<ChatServiceDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));

        // Add Identity
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 4;
        })
        .AddEntityFrameworkStores<ChatServiceDbContext>()
        .AddDefaultTokenProviders();

        // Add your services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMapperService, MapperService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IChatService, ChatService.Services.ChatService>();
        services.AddSingleton<ISharedMemoryConnectionDB, SharedMemoryConnectionsDB>();
        services.Configure<MinioSettings>(configuration.GetSection("Minio"));
;        services.AddSingleton<IS3Service>(sp => 
        {
            var minioSettings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
            return new MinioService(Options.Create(minioSettings));
        });

        // Add controllers
        services.AddControllers().AddApplicationPart(typeof(Program).Assembly).AddControllersAsServices();
        
        services.AddSignalR().AddHubOptions<ChatHub>(o => { o.ClientTimeoutInterval = TimeSpan.FromSeconds(60); });

        // Add authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<ChatHub>("/connectchat");
        });

        // Seed data
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            SeedData(userManager, roleManager).Wait();
        }
    }

    private async Task SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create roles
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create a test user
        var testUser = new User
        {
            UserName = "testUser",
            Email = "testuser@example.com",
            Nome = "Test User"
        };

        var userExists = await userManager.FindByNameAsync(testUser.UserName);
        if (userExists == null)
        {
            await userManager.CreateAsync(testUser, "123456");
            await userManager.AddToRoleAsync(testUser, "User");
        }
    }
}