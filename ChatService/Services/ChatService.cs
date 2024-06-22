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


    public ChatService(IHubContext<ChatHub> chatHubContext, ChatServiceDbContext context, UserManager<User> userManager,
        IMapperService mapperService)
    {
        _chatHubContext = chatHubContext;
        _context = context;
        _userManager = userManager;
        _mapperService = mapperService;
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

        ReadRequestDTO safeRequest = _mapperService.MapRequestToReadRequestDTO(requestFromDb);

        await _chatHubContext.Clients.User(user.Id).SendAsync("ReceiveRequest", safeRequest);

        return new SuccessResponse<ReadRequestDTO>(true, 200, "Requisição enviada com sucesso", safeRequest);
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
        
        ReadRequestDTO safeRequest = _mapperService.MapRequestToReadRequestDTO(request);
        await _context.SaveChangesAsync();
        
        await _chatHubContext.Clients.Users(safeRequest.RequesterId, safeRequest.RequestedId)
            .SendAsync("ReceiveRequestResponse", safeRequest, RequestClientResponse);
        
        request.Timestamp = DateTime.Now;


        return new SuccessResponse<ChatRequest>(true, 200, "Solicitação gerenciada com sucesso.", request);
    }

    public async Task<IResponse> GetActiveRequests(string UserId)
    {
        var requests = await _context.ChatRequests
            .Include(cr => cr.Requested)
            .Include(cr => cr.Requester)
            .Where(cr => cr.RequestedId == UserId | cr.RequesterId == UserId && cr.Rejected == false)
            .ToListAsync();

        List<ReadRequestDTO> safeRequests = new List<ReadRequestDTO>();
        safeRequests = requests.Select(request => _mapperService.MapRequestToReadRequestDTO(request)).ToList();
        return new SuccessResponse<List<ReadRequestDTO>>(true, 200, "Solicitações ativas encontradas", safeRequests);
    }
}