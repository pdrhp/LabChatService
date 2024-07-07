namespace ChatService.DTOs;

public record UpdateUserDto
{
    public string Username { get; init; }
    public string Nome { get; init; }
    public string Email { get; init; }
};