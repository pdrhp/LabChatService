using System.ComponentModel.DataAnnotations;

namespace ChatService.DTOs.ChatDTOs;

public class SendRequestDTO
{
    [Required]
    public string Username { get; set; }
}