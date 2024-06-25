using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ChatService.Data;
using ChatService.DTOs.ChatDTOs;
using ChatService.Interfaces;
using ChatService.Models;
using ChatService.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Hubs;

public class ChatHub: Hub
{
    private readonly IChatService _chatService;
    private UserManager<User> _userManager;
    private readonly ILogger<ChatHub> _logger;
    private readonly ISharedMemoryConnectionDB _sharedMemoryConnectionDB;
    public ChatHub(ChatServiceDbContext context, UserManager<User> userManager, ILogger<ChatHub> logger, IChatService chatService, ISharedMemoryConnectionDB sharedMemoryConnectionDB)
    {
        _chatService = chatService;
        _userManager = userManager;
        _logger = logger;
        _sharedMemoryConnectionDB = sharedMemoryConnectionDB;
    }
    
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var connectionId = Context.ConnectionId;
     
           
        _logger.LogInformation("Client connected, ConnectionID: {0}", userId);
        _logger.LogInformation("Client ID: {0}", connectionId);
        _sharedMemoryConnectionDB.Connections[userId] = connectionId;


        await Clients.Caller.SendAsync("ReceiveMessageFromServer", "Admin: " ,$"Connection established successfully! {userId}");

        await GetActiveConversations();

        await _chatService.ConnectionAlert(userId, true);
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.UserIdentifier;
        _sharedMemoryConnectionDB.Connections.TryRemove(userId, out string connectionId);
        _logger.LogInformation("Client disconnected, ConnectionID: {0}", connectionId);
        await _chatService.ConnectionAlert(userId, false);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToUser(SendMessageToUserDTO messageDto)
    {
        IResponse response = await _chatService.SendMessageToUser(messageDto);
        

        if (response is SuccessResponse<ChatMessage> successResponse)
        {
            ChatMessage message = successResponse.Data;
            IReadOnlyList<string> usersIds = new List<string> {messageDto.receiverId, messageDto.senderId};

            await Clients.Users(usersIds).SendAsync("MessageReceiveConfirmation", message.Id, true);
        }
    }

    public async Task GetActiveConversations()
    {
        string userId = Context.UserIdentifier;
        
        IResponse response = await _chatService.GetActiveRequests(userId);
        
        var activeRequests = ((SuccessResponse<List<ReadChatItemDto>>) response).Data;
        
        var firstRequest = activeRequests.FirstOrDefault();
        
        await Clients.User(userId).SendAsync("ReceiveActiveConversations", activeRequests);
    }

    public async Task Test()
    {
        await Clients.All.SendAsync("Teste", "admin", "Test");

        await Clients.All.SendAsync("TesteEspecifico", "admin", "Teste Especifico");
    }
}