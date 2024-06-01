using ChatService.DTOs;
using ChatService.Models;

namespace ChatService.Interfaces;

public interface IMapperService
{
    User MapUserDtoToUser(CreateUserDTO userDto);
}