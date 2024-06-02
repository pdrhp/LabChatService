namespace ChatService.DTOs;

public class CreateUserDTO
{
    public string Nome { get; set; }
    
    public string Username { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
    
    public string ConfirmPassword { get; set; }
    
    public string[] Roles { get; set; }
    
}