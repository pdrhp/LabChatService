namespace ChatService.DTOs;

public record ReadUserSessionDTO(string Id, string username, string nome, string timeStamp, List<string> Role, string Token);