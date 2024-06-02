using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ChatService.Data;
using ChatService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs;

public class ChatHub: Hub
{
    private readonly ChatServiceDbContext _context;
    private UserManager<User> _userManager;
    private readonly ILogger<ChatHub> _logger;
    private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new();
    public ChatHub(ChatServiceDbContext context, UserManager<User> userManager, ILogger<ChatHub> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        //
        // var teste2 = ConnectedUsers.TryGetValue(userId, out var connectionId);
        // var teste = ConnectedUsers.ContainsKey(userId);
        //
        // if (teste)
        // {
        //     _logger.LogInformation("User already connected: {0}", userId);
        //     Context.Abort();
        //     return;
        // }
        
        _logger.LogInformation("Client connected, ConnectionID: {0}", Context.ConnectionId);
        _logger.LogInformation("Client ID: {0}", Context.UserIdentifier);
        ConnectedUsers.TryAdd(userId, Context.ConnectionId);
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        
        var userId = Context.UserIdentifier;
        ConnectedUsers.TryRemove(userId, out _);
        
        _logger.LogInformation("Client disconnected, ConnectionID: {0}", Context.ConnectionId);
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToSpecificUser(string email, string message)
    {
        _logger.LogInformation("email: {0}", email);
        var user = await _userManager.FindByEmailAsync(email);
        var userId = Context.User.FindFirst(c => c.Type == "id").Value;
        var sender = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            await Clients.Caller.SendAsync("ReceiveMessageFromServer", "admin", "User not found");
            return;
        }
        
        await Clients.User(user.Id).SendAsync("ReceiveIndividualMessage", sender.Nome, message);
    }

    public async Task Test()
    {
        await Clients.All.SendAsync("Teste", "admin", "Test");

        await Clients.All.SendAsync("TesteEspecifico", "admin", "Teste Especifico");
    }
    
    

 

}