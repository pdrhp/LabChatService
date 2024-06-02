using System.Text.RegularExpressions;
using ChatService.Data;
using ChatService.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs;

public class ChatHub: Hub
{
    private readonly ChatServiceDbContext _context;
    
    public ChatHub(ChatServiceDbContext context)
    {
        _context = context;
    }

    public async Task SendMessage(int groupId, string message)
    {
    }

 

}