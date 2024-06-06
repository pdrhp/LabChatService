using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatService.Models;

public class ChatRequest
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public string RequesterId { get; set; }
    public virtual User Requester { get; set; }
    
    [ForeignKey("User")]
    public string RequestedId { get; set; }
    public virtual User Requested { get; set; }
    
    public DateTime Timestamp { get; set; }
    public bool Accepted { get; set; }
    public bool Rejected { get; set; }
}