namespace ChatService.DTOs;
public record CreateUserDTO
{
    public string Nome { get; init; }
    
    public string Username { get; init; }
    
    public string Password { get; init; }
    
    public string ConfirmPassword { get; init; }
    
    public ICollection<string> Roles { get; init; }
}