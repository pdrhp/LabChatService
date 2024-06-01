namespace ChatService.Interfaces;

public interface IResponse
{
    bool Flag { get; }
    int StatusCode { get; }
    string Message { get; }
}

