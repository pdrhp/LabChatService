using ChatService.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace ChatService.Interfaces;

public interface ITokenService
{
    ReadUserSessionDTO GenerateToken(CreateUserSessionDTO userSession);

    bool ValidateToken(string token);

    ReadUserSessionDTO GetUserSession(string token);
}