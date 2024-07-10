using System.Collections.Concurrent;
using ChatService.DTOs;
using ChatService.DTOs.ChatDTOs;
using ChatService.Models;

namespace ChatService.Interfaces;

public interface IMapperService
{
    User MapUserDtoToUser(SignUpUserDTO userDto);
    User MapUserDtoToUser(CreateUserDTO userDto);
    ReadChatItemDto MapRequestToReadRequestDTO(ChatRequest request,
        ConcurrentDictionary<string, string> activeConnections);
}