using System.Collections;
using ChatService.Data;
using ChatService.DTOs.ChatDTOs;
using ChatService.Hubs;
using ChatService.Interfaces;
using ChatService.Models;
using ChatService.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Services;

public class ChatService : IChatService
{
    private readonly IHubContext<ChatHub> _chatHubContext;
    private readonly ChatServiceDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IMapperService _mapperService;
    private readonly ISharedMemoryConnectionDB _sharedMemoryConnectionDB;


    public ChatService(IHubContext<ChatHub> chatHubContext, ChatServiceDbContext context, UserManager<User> userManager,
        IMapperService mapperService, ISharedMemoryConnectionDB sharedMemoryConnectionDb)
    {
        _chatHubContext = chatHubContext;
        _context = context;
        _userManager = userManager;
        _mapperService = mapperService;
        _sharedMemoryConnectionDB = sharedMemoryConnectionDb;
    }
    
    public async Task<IResponse> SendRequest(SendRequestDTO dto, string RequesterId)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return new ErrorResponse(false, 404, "Usuário não encontrado");
        }

        var requestExists = _context.ChatRequests.Any(cr =>
            cr.RequestedId == user.Id && cr.RequesterId == RequesterId && cr.Accepted == false && cr.Rejected == false);

        if (requestExists)
        {
            return new ErrorResponse(false, 400, "Requisição já enviada");
        }


        var request = new ChatRequest
        {
            RequesterId = RequesterId,
            RequestedId = user.Id,
            Timestamp = DateTime.Now,
            Accepted = false,
            Rejected = false
        };

        await _context.ChatRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        var requestFromDb = _context.ChatRequests.Include(cr => cr.Requester)
            .FirstOrDefault(cr =>
                cr.RequestedId == user.Id && cr.RequesterId == RequesterId && cr.Accepted == false &&
                cr.Rejected == false);
        
        var connectedUsers = _sharedMemoryConnectionDB.Connections;

        
        ReadChatItemDto safeChatItem = _mapperService.MapRequestToReadRequestDTO(requestFromDb, connectedUsers);

        await _chatHubContext.Clients.User(user.Id).SendAsync("ReceiveRequest", safeChatItem);

        return new SuccessResponse<ReadChatItemDto>(true, 200, "Requisição enviada com sucesso", safeChatItem);
    }

    public async Task<IResponse> SendMessageToUser(SendMessageToUserDTO messageDto)
    {
        ChatMessage chatMessage = new ChatMessage
        {
            ReceiverId = messageDto.receiverId,
            SenderId = messageDto.senderId!,
            Message = messageDto.message,
            ChatRequestId = messageDto.requestId,
            Timestamp = DateTime.Now
        };

        var ChatMessageEntityEntry = await _context.ChatMessages.AddAsync(chatMessage);
        
        await _chatHubContext.Clients.Users(messageDto.receiverId, messageDto.senderId).SendAsync("ReceiveMessage", ChatMessageEntityEntry.Entity);

        await _context.SaveChangesAsync();
        
        return new SuccessResponse<ChatMessage>(true, 200, "Mensagem enviada com sucesso", ChatMessageEntityEntry.Entity);
    }

    public async Task<IResponse> ConnectionAlert(string UserId, bool IsConnected)
    {
        var activeConversationsWithThisUser = await _context.ChatRequests
            .Where(cr => cr.RequesterId == UserId | cr.RequestedId == UserId && cr.Rejected == false)
            .Select(cr => new { cr.RequesterId, cr.RequestedId })
            .ToListAsync();
        
        var ids = activeConversationsWithThisUser.SelectMany(cr => new List<string> { cr.RequesterId, cr.RequestedId }).Distinct().ToList();
        ids = ids.Where(id => id != UserId).ToList();
        await _chatHubContext.Clients.Users(ids).SendAsync("ReceiveConnectionAlert", UserId, IsConnected);
        return new SuccessResponse<bool>(true, 200, "Alerta de conexão enviado com sucesso", IsConnected);
    }


    public async Task<IResponse> ManageRequest(int RequestId, bool RequestClientResponse)
    {
        var request = _context.ChatRequests
            .Include(cr => cr.Requester)
            .Include(cr => cr.Requested)
            .FirstOrDefault(cr => cr.Id == RequestId && cr.Accepted == false && cr.Rejected == false);

        if (request == null)
        {
            return new ErrorResponse(false, 404, "Solicitação não encontrada");
        }

        request.Accepted = RequestClientResponse;
        request.Rejected = !RequestClientResponse;
        request.Timestamp = DateTime.Now;
        
        
        var connectedUsers = _sharedMemoryConnectionDB.Connections;
        
        ReadChatItemDto safeChatItem = _mapperService.MapRequestToReadRequestDTO(request, connectedUsers);
        await _context.SaveChangesAsync();
        
        await _chatHubContext.Clients.Users(safeChatItem.RequesterId, safeChatItem.RequestedId)
            .SendAsync("ReceiveRequestResponse", safeChatItem, RequestClientResponse);
        
        return new SuccessResponse<ChatRequest>(true, 200, "Solicitação gerenciada com sucesso.", request);
    }

    public async Task<IResponse> GetActiveRequests(string UserId)
    {
        var requests = await _context.ChatRequests
            .Include(cr => cr.Requested)
            .Include(cr => cr.Requester)
            .Include(cr => cr.Messages)
            .Where(cr => cr.RequestedId == UserId | cr.RequesterId == UserId && cr.Rejected == false)
            .ToListAsync();
        
        var connectedUsers = _sharedMemoryConnectionDB.Connections;

        
        
        List<ReadChatItemDto> safeRequests = new List<ReadChatItemDto>();
        safeRequests = requests.Select(request => _mapperService.MapRequestToReadRequestDTO(request, connectedUsers)).ToList();
        return new SuccessResponse<List<ReadChatItemDto>>(true, 200, "Solicitações ativas encontradas", safeRequests);
    }
    
    
}