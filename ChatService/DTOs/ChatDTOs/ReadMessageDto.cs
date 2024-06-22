namespace ChatService.DTOs.ChatDTOs;

public record ReadMessageDto
{
    public int Id { get; init; }
    public string SenderId { get; init; }
    public string ReceiverId { get; init; }
    public string Message { get; init; }
    public DateTime Timestamp { get; init; }
};