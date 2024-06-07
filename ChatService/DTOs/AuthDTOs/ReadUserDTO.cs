namespace ChatService.DTOs;

public record ReadUserDTO
{
    public string Nome { get; init; }
    public string Email { get; init; }
    public string Id { get; init; }
    public string UserName { get; init; }
}