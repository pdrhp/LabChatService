using ChatService.DTOs;
using ChatService.Helper;
using ChatService.Interfaces;
using Microsoft.AspNet.SignalR;
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var response = await _userService.GetAllUsers();
        
        if (response.Flag == false)
        {
            return ResponseHelper.HandleError(this, response);
        }
        
        return Ok(response);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> SignUpUser([FromBody] SignUpUserDTO dto)
    {
        var response = await _userService.SignUpUser(dto);

        if (response.Flag == false)
        {
            ResponseHelper.HandleError(this, response);
        }
        
        return Created("Sucesso", response);
    }
    
    [HttpGet("{userId}/profilePicture")]
    public async Task<IActionResult> GetProfilePicture(string userId)
    {
        var result = await _userService.GetProfilePicture(userId);
        
        if (result.Flag == false)
        {
            ResponseHelper.HandleError(this, result);
        }
        
        return Ok(result);
    }
    
    [HttpPost("{userId}/updateProfilePicture")]
    public async Task<IActionResult> UpdateProfilePicture(string userId, [FromForm] IFormFile file)
    {
        var result = await _userService.UpdateProfilePicture(userId, file);
        
        if (result.Flag == false)
        {
            ResponseHelper.HandleError(this, result);
        }

        return Ok(result);
    }
    
    
}