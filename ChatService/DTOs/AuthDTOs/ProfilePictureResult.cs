namespace ChatService.DTOs;

public record ProfilePictureResult
{
    public byte[] ImageData { get; init; }
    public string ImageType { get; init; }
}