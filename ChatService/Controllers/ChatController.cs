using System.Net;
using ChatService.DTOs.ChatDTOs;
using ChatService.Hubs;
using ChatService.Interfaces;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController: ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ILogger<ChatController> logger, IChatService chatService)
    {
        _logger = logger;
        _chatService = chatService;
    }
    
    
    [HttpPost("sendRequest")]
    public async Task<IActionResult> SendRequest([FromBody] SendRequestDTO dto)
    {
        var id = HttpContext.User.FindFirst(c => c.Type == "id").Value;
        var response = await _chatService.SendRequest(dto, id);
        
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost("manageRequest/{requestId}")]
    public async Task<IActionResult> ManageRequest(int requestId, [FromBody] ManageRequestDTO dto)
    {
        
        var response = await _chatService.ManageRequest(requestId,  dto.RequestClientResponse);
        return StatusCode(response.StatusCode, response);
    }
}