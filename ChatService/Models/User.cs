using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ChatService.Models;

public class User : IdentityUser
{
    
    [MaxLength(100)]
    public string Nome { get; set; }
    
    
    [Url]
    public string? ProfilePictureUrl { get; set; }
    
    public ICollection<ChatRequest> RequestsAsRequester { get; set; }
    public ICollection<ChatRequest> RequestsAsRequested { get; set; }
    
    public ICollection<ChatMessage> MessagesAsSender { get; set; }
    public ICollection<ChatMessage> MessagesAsReceiver { get; set; }
    public User() : base() { }
}