using ChatService.Interfaces;

namespace ChatService.Responses;

public record SuccessResponse(bool Flag, int StatusCode, string Message) : IResponse;

public record SuccessResponse<T>(bool Flag, string Message, int StatusCode, T Data) : IResponse;

public record ErrorResponse(bool Flag, int StatusCode, string Message) : IResponse;

