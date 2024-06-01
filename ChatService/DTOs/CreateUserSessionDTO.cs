namespace ChatService.DTOs;

public record CreateUserSessionDTO(string? Id, string? username, string? nome, List<string>? Role);