﻿using ChatService.DTOs;

namespace ChatService.Interfaces;

public interface IUserService
{
    Task<IResponse> SignUpUser(SignUpUserDTO user);
    Task<IResponse> LogInUser(LoginUserDto user, HttpContext context);
    Task<IResponse> GetRole(string id);
    IResponse ValidateToken(string token);
    Task<IResponse> CreateRoles();
    Task<IResponse> GetAllUsers();
    Task<IResponse> CreateUser(CreateUserDTO userDto);
    Task<IResponse> VerifySession(HttpContext context);
    Task<IResponse> SignOutUser(HttpContext context);
    Task<IResponse> GetProfilePicture(string userId);
    Task<IResponse> UpdateProfilePicture(string userId, IFormFile file);
    void SetTokensInsideCookie(string token, HttpContext context);
}