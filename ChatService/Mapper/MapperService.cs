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

    public ReadRequestDTO MapRequestToReadRequestDTO(ChatRequest request)
    {
        return new ReadRequestDTO
        {
            Accepted = request.Accepted,
            Rejected = request.Rejected,
            RequestedId = request.RequestedId,
            Requested = new ReadUserDTO
            {
                Email = request.Requested.Email,
                Id = request.Requested.Id,
                Nome = request.Requested.Nome,
                UserName = request.Requested.UserName
            },
            RequesterId = request.RequesterId,
            Requester = new ReadUserDTO
            {
                Email = request.Requester.Email,
                Id = request.Requester.Id,
                Nome = request.Requester.Nome,
                UserName = request.Requester.UserName
            },
            Timestamp = request.Timestamp
        };
    }
}