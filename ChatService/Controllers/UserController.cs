using ChatService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("{userId}/profilePicture")]
    public async Task<IActionResult> GetProfilePicture(string userId)
    {
        var result = await _userService.GetProfilePicture(userId);
        return File(result.ImageData, result.ImageType);
    }
    
    [HttpPost("{userId}/updateProfilePicture")]
    public async Task<IActionResult> UpdateProfilePicture(string userId, [FromForm] IFormFile file)
    {
        var result = await _userService.UpdateProfilePicture(userId, file);
        return File(result.ImageData, result.ImageType);
    }
}