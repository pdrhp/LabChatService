using System.Collections.Concurrent;
using ChatService.DTOs;
using ChatService.DTOs.ChatDTOs;
using ChatService.Interfaces;
using ChatService.Models;

namespace ChatService.Mapper;

public class MapperService : IMapperService
{
    public User MapUserDtoToUser(CreateUserDTO userDto)
    {
        return new User
        {
            UserName = userDto.Username,
            Nome = userDto.Nome,
            Email = userDto.Email
        };
    }

    public ReadChatItemDto MapRequestToReadRequestDTO(ChatRequest request, ConcurrentDictionary<string, string> activeConnections)
    {
        var requesterOnline = activeConnections.ContainsKey(request.RequesterId);
        var requestedOnline = activeConnections.ContainsKey(request.RequestedId);
        
        
        return new ReadChatItemDto
        {
            Id = request.Id,
            Accepted = request.Accepted,
            Rejected = request.Rejected,
            RequestedId = request.RequestedId,
            Requested = new ReadUserDTO
            {
                Email = request.Requested.Email,
                Id = request.Requested.Id,
                Nome = request.Requested.Nome,
                UserName = request.Requested.UserName,
                online = requestedOnline
            },
            RequesterId = request.RequesterId,
            Requester = new ReadUserDTO
            {
                Email = request.Requester.Email,
                Id = request.Requester.Id,
                Nome = request.Requester.Nome,
                UserName = request.Requester.UserName,
                online = requesterOnline
            },
            Timestamp = request.Timestamp,
            Messages = request.Messages?.Select(m => new ReadMessageDto
            {
                Id = m.Id,
                Message = m.Message,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Timestamp = m.Timestamp
            }).ToList()
        };
    }
}