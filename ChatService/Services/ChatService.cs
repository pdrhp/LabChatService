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
    

    public ChatService(IHubContext<ChatHub> chatHubContext, ChatServiceDbContext context, UserManager<User> userManager, ILogger<ChatService> logger)
    {
        _chatHubContext = chatHubContext;
        _context = context;
        _userManager = userManager;
    }

    
    public async Task<IResponse> SendRequest(SendRequestDTO dto, string RequesterId)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if(user == null){
            return new ErrorResponse(false, 404, "Usuário não encontrado");
        }
        
        var requestExists = _context.ChatRequests.Any(cr => cr.RequestedId == user.Id && cr.RequesterId == RequesterId && cr.Accepted == false && cr.Rejected == false);
        
        if(requestExists){
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
            .FirstOrDefault(cr => cr.RequestedId == user.Id && cr.RequesterId == RequesterId && cr.Accepted == false && cr.Rejected == false);
        
        await _chatHubContext.Clients.User(user.Id).SendAsync("ReceiveRequest", requestFromDb);
        
        return new SuccessResponse<ChatRequest>(true, 200, "Requisição enviada com sucesso", requestFromDb);
    }


    public async Task<IResponse> ManageRequest(string RequestId, string RequesterId, bool RequestClientResponse)
    {
        var request = _context.ChatRequests.Include(cr => cr.Requester)
            .FirstOrDefault(cr => cr.RequestedId == RequesterId && cr.Accepted == false && cr.Rejected == false);
        
        if(request == null){
            return new ErrorResponse(false, 404, "Solicitação não encontrada");
        }

        if (RequestClientResponse)
        {
            request.Accepted = true;
            await _chatHubContext.Clients.User(request.RequesterId).SendAsync("ReceiveRequestResponse", request, true);
        }
        else
        {
            request.Rejected = true;
            await _chatHubContext.Clients.User(request.RequesterId).SendAsync("ReceiveRequestResponse", request, false);
        }
        
        request.Timestamp = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        return new SuccessResponse<ChatRequest>(true, 200, "Solicitação gerenciada com sucesso.", request);
    }
}