using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatService.Models;

public class ChatMessage
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string UserId { get; set; }
    [Required]
    [MaxLength(600)]
    public string Message { get; set; }
    [Required]
    public DateTime Timestamp { get; set; }
    [ForeignKey("ChatGroup")]
    public int ChatGroupId { get; set; }
    public ChatGroup ChatGroup { get; set; }
}