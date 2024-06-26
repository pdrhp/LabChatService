﻿using ChatService.DTOs.ChatDTOs;

namespace ChatService.Interfaces;

public interface IChatService
{
    Task<IResponse> SendRequest(SendRequestDTO dto, string RequesterId);
    
    Task<IResponse> ManageRequest(int RequestId,  bool RequestClientResponse);

    Task<IResponse> GetActiveRequests(string UserId);

    Task<IResponse> SendMessageToUser(SendMessageToUserDTO messageDto);
    Task<IResponse> ConnectionAlert(string UserId, bool IsConnected);
}