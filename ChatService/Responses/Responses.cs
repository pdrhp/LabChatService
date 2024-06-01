using ChatService.Interfaces;

namespace ChatService.Responses;

public record SuccessResponse(bool Flag, int StatusCode, string Message) : IResponse;

public record SuccessResponse<T>(bool Flag, int StatusCode, string Message,  T Data) : IResponse;

public record ErrorResponse(bool Flag, int StatusCode, string Message) : IResponse;

