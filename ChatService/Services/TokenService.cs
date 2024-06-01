using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatService.DTOs;
using ChatService.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ChatService.Services;

public class TokenService : ITokenService
{
    
    private IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key:SymmetricSecurityKey"])),
            ValidateAudience = false,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        };
    }
    
    public ReadUserSessionDTO GenerateToken(CreateUserSessionDTO userSession)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("id", userSession.Id),
            new Claim("username", userSession.username),
            new Claim("loginTimeStamp", DateTime.UtcNow.ToString())
        });

        foreach (var role in userSession.Role)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
        
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key:SymmetricSecurityKey"]));
        
        var signInCrendentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
        (
            expires: DateTime.Now.AddHours(5),
            claims: identity.Claims,
            signingCredentials: signInCrendentials
        );

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new ReadUserSessionDTO(userSession.Id, userSession.username, DateTime.UtcNow.ToString(),
            userSession.Role, tokenString);
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public ReadUserSessionDTO GetUserSession(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

        var id = principal.FindFirst("id").Value;
        var username = principal.FindFirst("username").Value;
        var loginTimeStamp = principal.FindFirst("loginTimeStamp").Value;
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        
        return new ReadUserSessionDTO(id, username, loginTimeStamp, roles, token);
    }
}