using Microsoft.AspNetCore.SignalR;

namespace ChatService.Providers;

public class UserIdProvider: IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirst("id").Value;
    }
}