namespace ChatService.DTOs;

public record ReadUserSessionDTO(string? Id, string? username, string? nome, string? email, string? timeStamp, List<string>? Role, string Token);