using ChatService.Models;
using Microsoft.AspNetCore.Identity;

namespace ChatService.Services;

public class UserService
{
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private RoleManager<IdentityRole> _roleManager;
}