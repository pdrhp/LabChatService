using System.Net;
using System.Net.Http.Json;
using ChatService.DTOs;
using ChatService.Interfaces;
using ChatService.Responses;
using ChatServiceTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace ChatServiceTests.Integration.Auth;

[Collection("Sequential")]
public class AuthIntegrationTests : IntegrationTestBase
{
    
    public AuthIntegrationTests(ChatServiceFactory factory) : base(factory) { }
    
    [Fact]
    public async Task Register_SignUpUser_WhenUserDtoIsValid()
    {

        // Arrange
        var _userService = GetService<IUserService>();
        var userDto = new SignUpUserDTO()
        {
            Nome = "Pedro Henrique",
            Username = "pdrhpconta1",
            Password = "123456",
            ConfirmPassword = "123456"
        };
        
        // Act
        var result = await _userService.SignUpUser(userDto);
        
        
        // Assert
        result.Flag.Should().BeTrue();
        result.StatusCode.Should().Be(201);
    }
    
    
    [Fact]
    public async Task Register_SignUpUser_WhenUserDtoIsInvalidWithWrongRePassword()
    {
        // Arrange
        var _userService = GetService<IUserService>();
        var userDto = new SignUpUserDTO()
        {
            Nome = "Pedro Henrique",
            Username = "pdrhpconta1testeinvalido",
            Password = "123456",
            ConfirmPassword = "1234567"
        };
        
        // Act
        var result = await _userService.SignUpUser(userDto);
        
        
        // Assert
        result.Flag.Should().BeFalse();
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Login_SignInUser_WhenUserDtoIsValid()
    {
        // Arrange
        var _userService = GetService<IUserService>();
        var userDto = new LoginUserDto()
        {
            Username = "testUser",
            Password = "123456"
        };
        
        // Act
        var response = Client.PostAsJsonAsync("/auth/signin", userDto);
        
        
        // Assert
        var responseContent = await response.Result.Content.ReadFromJsonAsync<SuccessResponse<ReadUserDTO>>();
        
        responseContent.Message.Should().Be("Usuário logado com sucesso!");
        responseContent.Flag.Should().BeTrue();
        response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Login_SignInUser_WhenUserDtoIsInvalidWithWrongPassword()
    {
        // Arrange
        var _userService = GetService<IUserService>();
        var userDto = new LoginUserDto()
        {
            Username = "testUser",
            Password = "1234567"
        };
        
        // Act
        var response = Client.PostAsJsonAsync("/auth/signin", userDto);
        
        
        // Assert
        var responseContent = await response.Result.Content.ReadFromJsonAsync<ErrorResponse>();
        
        responseContent.Message.Should().Be("Senha incorreta");
        responseContent.Flag.Should().BeFalse();
        response.Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Login_SignInUser_WhenUsernameDoesNotExist()
    {
        // Arrange
        var _userService = GetService<IUserService>();
        var userDto = new LoginUserDto()
        {
            Username = "testeUSuarioInexistente",
            Password = "123456"
        };
        
        // Act
        var response = Client.PostAsJsonAsync("/auth/signin", userDto);
        var responseContent = await response.Result.Content.ReadFromJsonAsync<SuccessResponse<ReadUserDTO>>();
        
        
        // Assert
        responseContent.Message.Should().Be("Usuário não encontrado");
        responseContent.Flag.Should().BeFalse();
        response.Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
}