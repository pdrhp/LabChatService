namespace ChatService.DTOs;

public record CreateUserSessionDTO(string? Id, string? username, string? nome, string? email, List<string>? Role);