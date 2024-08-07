﻿namespace ChatService.DTOs;

public record SignUpUserDTO
{
    public string Nome { get; init; }
    
    public string Username { get; init; }
    
    public string Password { get; init; }
    
    public string ConfirmPassword { get; init; }
}

