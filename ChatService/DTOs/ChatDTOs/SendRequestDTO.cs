using System.ComponentModel.DataAnnotations;

namespace ChatService.DTOs.ChatDTOs;

public class SendRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}