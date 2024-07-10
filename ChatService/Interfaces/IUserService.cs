using ChatService.DTOs;

namespace ChatService.Interfaces;

public interface IUserService
{
    Task<IResponse> SignUpUser(SignUpUserDTO user);
    Task<IResponse> LogInUser(LoginUserDto user, HttpContext context);
    Task<IResponse> GetRole(string id);
    IResponse ValidateToken(string token);
    Task<IResponse> CreateRoles();
    Task<IResponse> VerifySession(HttpContext context);
    Task<IResponse> SignOutUser(HttpContext context);
    // Task<ProfilePictureResult> GetProfilePicture(string userId);
    // Task<ProfilePictureResult> UpdateProfilePicture(string userId, IFormFile file);
    void SetTokensInsideCookie(string token, HttpContext context);
}