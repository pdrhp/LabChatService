namespace ChatService.DTOs.ChatDTOs;

public record ReadRequestDTO
{
    public bool Accepted { get; init; }
    public bool Rejected { get; init; }
    public string RequesterId { get; init; }
    public ReadUserDTO Requester { get; init; }
    public string RequestedId { get; init; }
    public ReadUserDTO Requested { get; init; }
    public DateTime Timestamp { get; init; }
}