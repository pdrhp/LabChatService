using ChatService.DTOs;
using ChatService.Helper;
using ChatService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController: ControllerBase
{
    private IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("signUp")]
    public async Task<IActionResult> SignUpUser([FromBody] SignUpUserDTO dto)
    {
        var response = await _userService.SignUpUser(dto);

        if (response.Flag == false)
        {
            return ResponseHelper.HandleError(this, response);
        }
        
        return Created("Sucesso", response);
    }

    [HttpPost("signIn")]
    public async Task<IActionResult> SignInUser([FromBody] LoginUserDto dto)
    {
        var response = await _userService.LogInUser(dto, HttpContext);
        
        if (response.Flag == false)
        {
            return ResponseHelper.HandleError(this, response);
        }
        
        return Ok(response);
    }

    [HttpPost("signOut")]
    public async Task<IActionResult> SignOutUser()
    {
        var response = await _userService.SignOutUser(HttpContext);
        
        if (response.Flag == false)
        {
            return ResponseHelper.HandleError(this, response);
        }
        
        return Ok(response);
    }

    [HttpGet("verifySession")]
    [Authorize]
    public async Task<IActionResult> VerifySession()
    {
        var response = await _userService.VerifySession(HttpContext);
        
        if (response.Flag == false)
        {
            return ResponseHelper.HandleError(this, response);
        }
        
        return Ok(response);
    }

    [HttpGet("validateToken")]
    public IActionResult ValidateToken([FromQuery] string token)
    {
        var response = _userService.ValidateToken(token);
        
        if (response.Flag == false)
        {
           return  ResponseHelper.HandleError(this, response);
        }
        
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("createRoles")]
    public async Task<IActionResult> CreateRoles()
    {
        var rolesResponse = await _userService.CreateRoles();

        if (rolesResponse.Flag == false)
        {
            return BadRequest(rolesResponse);
        }
        
        return Created("Sucesso", rolesResponse);
    }
    
}