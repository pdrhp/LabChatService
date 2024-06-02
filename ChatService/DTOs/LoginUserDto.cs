using System.ComponentModel.DataAnnotations;

namespace ChatService.DTOs;

public record LoginUserDto
{
    [Required]
    public string Email { get; init; }
    
    [Required]
    public string Password { get; init; }
}