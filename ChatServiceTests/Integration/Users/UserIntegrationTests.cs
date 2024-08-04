using ChatService.Data;
using ChatService.DTOs;
using ChatService.Interfaces;
using ChatService.Models;
using ChatService.Responses;
using ChatServiceTests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ChatServiceTests.Integration.Users;

[Collection("Sequential")]
public class UserIntegrationTests : IntegrationTestBase
{
    private readonly IUserService _userService;
    private readonly ChatServiceDbContext _context;

    public UserIntegrationTests(ChatServiceFactory factory) : base(factory)
    {
        _userService = GetService<IUserService>();
        _context = GetService<ChatServiceDbContext>();
    }


    [Fact]
    public async Task Create_CreateUser_WhenUserDtoIsValidWithRoleUser()
    {
        // Arrange
        var userDto = new CreateUserDTO()
        {
            Nome = "Pedro Henrique",
            Username = "pdrhpUsuario3",
            Password = "123456",
            ConfirmPassword = "123456",
            Roles = ["User"]
        };
        
        // Act
        var result = (await _userService.CreateUser(userDto)) as SuccessResponse<User>;
        
        // Assert
        result.Flag.Should().BeTrue();
        result.Message.Should().Be("Usuário cadastrado com sucesso!");
        result.StatusCode.Should().Be(201);
    }
    
    [Fact]
    public async Task Create_CreateUser_WhenUserDtoIsValidWithRoleAdmin()
    {
        // Arrange
        var userDto = new CreateUserDTO()
        {
            Nome = "Pedro Henrique",
            Username = "pdrhpUsuario4",
            Password = "123456",
            ConfirmPassword = "123456",
            Roles = ["Admin"]
        };
        
        // Act
        var result = (await _userService.CreateUser(userDto)) as SuccessResponse<User>;
        
        // Assert
        result.Flag.Should().BeTrue();
        result.Message.Should().Be("Usuário cadastrado com sucesso!");
        result.StatusCode.Should().Be(201);
    }
    
    [Fact]
    public async Task Create_CreateUser_WhenUserDtoIsInvalidWithWrongRePassword()
    {
        // Arrange
        var userDto = new CreateUserDTO()
        {
            Nome = "Pedro Henrique",
            Username = "pdrhpUsuario5",
            Password = "123456",
            ConfirmPassword = "1234567",
            Roles = ["User"]
        };
        
        // Act
        var result = await _userService.CreateUser(userDto);
        
        // Assert
        result.Flag.Should().BeFalse();
        result.Message.Should().Be("As senhas não coincidem");
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetUser_GetAllUsers_WhenThereAreUsers()
    {
        // Arrange
        
        // Act
        var result = (await _userService.GetAllUsers()) as SuccessResponse<IEnumerable<ReadUserDTO>>;
        
        // Assert
        result.Flag.Should().BeTrue();
        result.Message.Should().Be("Usuários encontrados!");
        result.StatusCode.Should().Be(200);
    }
    
    [Fact]
    public async Task GetUser_GetAllUsers_WhenThereAreNoUsers()
    {
        // Arrange
        _context.Users.RemoveRange(_context.Users.ToList());
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _userService.GetAllUsers();
        
        // Assert
        result.Message.Should().Be("Nenhum usuário encontrado!");
        result.Flag.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }
    

}