using ChatService.DTOs;
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
}