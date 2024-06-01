namespace ChatService.DTOs;

public record ReadUserSessionDTO(string? Id, string? username, string? nome, List<string>? Role, string Token);