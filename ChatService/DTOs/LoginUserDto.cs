using System.ComponentModel.DataAnnotations;

namespace ChatService.DTOs;

public record LoginUserDto
{
    [Required]
    public string Username { get; init; }
    
    [Required]
    public string Password { get; init; }
}