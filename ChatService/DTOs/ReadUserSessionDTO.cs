namespace ChatService.DTOs;

public record ReadUserSessionDTO(string? Id, string? username, string? timeStamp, List<string>? Role, string Token);