namespace ChatService.DTOs;

public record CreateUserSessionDTO(string? Id, string? username, string? timeStamp, List<string>? Role);