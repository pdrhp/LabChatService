namespace ChatService.DTOs.ChatDTOs;

public record ManageRequestDTO
{
    public string RequesterId { get; init; }
    public bool RequestClientResponse { get; init; }
}