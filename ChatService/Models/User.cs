using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ChatService.Models;

public class User : IdentityUser
{
    
    [MaxLength(100)]
    public string Nome { get; set; }
    
    public byte[]? ProfilePicture { get; set; }
    public string? ProfilePictureType { get; set; }
    public ICollection<ChatRequest> RequestsAsRequester { get; set; }
    public ICollection<ChatRequest> RequestsAsRequested { get; set; }
    public User() : base() { }
}