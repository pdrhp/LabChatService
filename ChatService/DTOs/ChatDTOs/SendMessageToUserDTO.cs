namespace ChatService.DTOs.ChatDTOs;

public record SendMessageToUserDTO
{
    public string receiverId { get; init; }
    public string senderId { get; init; }
    public string message { get; init; }
    public int requestId { get; init; }
};