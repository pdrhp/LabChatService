using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatService.Models;

public class ChatMessage
{
    [Key]
    public int Id { get; set; }
    
    public int ChatRequestId { get; set; }
    public virtual ChatRequest ChatRequest { get; set; }
    
    [Required]
    public string SenderId { get; set; }
    public virtual User Sender { get; set; }
    [Required]
    public string ReceiverId { get; set; }
    public virtual User Receiver { get; set; }
    [Required]
    [MaxLength(600)]
    public string Message { get; set; }
    [Required]
    public DateTime Timestamp { get; set; }
}