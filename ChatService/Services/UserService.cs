using ChatService.DTOs;
using ChatService.Interfaces;
using ChatService.Models;
using ChatService.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Services;

public class UserService: IUserService
{
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private RoleManager<IdentityRole> _roleManager;
    private IMapperService _mapper;
    private ITokenService _tokenService;
    private readonly IS3Service _s3Service;
    
    
    public UserService(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        IMapperService mapper,
        ITokenService tokenService,
        RoleManager<IdentityRole> roleManager,
        IS3Service s3Service)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _tokenService = tokenService;
        _s3Service = s3Service;
    }

    public async Task<IResponse> SignUpUser(SignUpUserDTO userDto)
    {
        if (userDto is null)
            return new ErrorResponse(false, 400, "Usuário não pode ser nulo");

        User newUser = _mapper.MapUserDtoToUser(userDto);

        if (userDto.Password != userDto.ConfirmPassword)
            return new ErrorResponse(false, 400, "As senhas não coincidem");

        IdentityResult result = await _userManager.CreateAsync(newUser, userDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return new ErrorResponse(false, 400, $"Falha ao cadastrar usuário! Erros: {string.Join(", ", errors)}");
        }
        
        await _userManager.AddToRoleAsync(newUser, "User");

        ReadUserDTO mappedUser = _mapper.MapUserToReadUserDTO(newUser);

        return new SuccessResponse<ReadUserDTO>(true, 201, "Usuário cadastrado com sucesso!", mappedUser);
    }
    
    public async Task<IResponse> CreateUser(CreateUserDTO userDto)
    {
        if (userDto is null)
            return new ErrorResponse(false, 400, "Usuário não pode ser nulo");

        User newUser = _mapper.MapUserDtoToUser(userDto);

        if (userDto.Password != userDto.ConfirmPassword)
            return new ErrorResponse(false, 400, "As senhas não coincidem");

        IdentityResult result = await _userManager.CreateAsync(newUser, userDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return new ErrorResponse(false, 400, $"Falha ao cadastrar usuário! Erros: {string.Join(", ", errors)}");
        }
        
        await _userManager.AddToRolesAsync(newUser, userDto.Roles);

        return new SuccessResponse<User>(true, 201, "Usuário cadastrado com sucesso!", newUser);
    }

    public async Task<IResponse> LogInUser(LoginUserDto userDto, HttpContext context)
    {
        var appUser = await _userManager.FindByNameAsync(userDto.Username);
        
        if (appUser is null)
            return new ErrorResponse(false, 400, "Usuário não encontrado");
        
        var signInResult = await _signInManager.PasswordSignInAsync(appUser, userDto.Password, false, false);
        
        if (!signInResult.Succeeded)
            return new ErrorResponse(false, 400, "Senha incorreta");

        var userRoles = await _userManager.GetRolesAsync(appUser);
        
        CreateUserSessionDTO userSession = new CreateUserSessionDTO(appUser.Id, appUser.UserName, appUser.Nome, userRoles.ToList());

        ReadUserSessionDTO loggedUser = _tokenService.GenerateToken(userSession);
        
        SetTokensInsideCookie(loggedUser.Token, context);
        
        return new SuccessResponse<ReadUserSessionDTO>(true, 200, "Usuário logado com sucesso!", loggedUser);
    }
    
    public async Task<IResponse> SignOutUser(HttpContext context)
    {
        context.Response.Cookies.Delete("accessToken");

        await _signInManager.SignOutAsync();

        return new SuccessResponse(true, 200, "Usuário deslogado com sucesso!");
    }

    public async Task<IResponse> GetProfilePicture(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user is null)
            return new ErrorResponse(false, 404, "Usuário não encontrado");
        
        if(user.ProfilePictureUrl is null)
            return new ErrorResponse(false, 404, "Imagem de perfil não encontrada");
        
        return new SuccessResponse<string>(true, 200, "Imagem de perfil encontrada!", user.ProfilePictureUrl);
    }

    public async Task<IResponse> UpdateProfilePicture(string userId, IFormFile file)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ErrorResponse(false, 404, "Usuário não encontrado!");
        }
    
        Stream stream = file.OpenReadStream();
        

        string imageUrl = await _s3Service.UploadFileAsync("public", $"{user.UserName}_picture", stream,
            stream.Length, file.ContentType);

        user.ProfilePictureUrl = "https://" + imageUrl;
            
        var result = await _userManager.UpdateAsync(user);
    
        if (!result.Succeeded)
        {
            return new ErrorResponse(false, 400, "Erro ao atualizar imagem de perfil!");
        }


        return new SuccessResponse<string>(true, 200, "Imagem de perfil atualizada com sucesso!", imageUrl);
    }

    public async Task<IResponse> GetRole(string id)
    {
        throw new NotImplementedException();
    }

    public IResponse ValidateToken(string token)
    {
        bool tokenValidation = _tokenService.ValidateToken(token);

        if (tokenValidation == false)
        {
            return new ErrorResponse(false, 401, "Token inválido!");
        }

        return new SuccessResponse<string>(true, 200, "Token validado com sucesso!", token);
    }

    public async Task<IResponse> CreateRoles()
    {
        string[] roles = new string[] { "User", "Admin" };

        foreach (var role in roles)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(role);
            
            if(!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = role});
            }
        }
        
        return new SuccessResponse<string[]>(true, 201, "Roles criadas com sucesso!", roles);
    }

    public async Task<IResponse> VerifySession(HttpContext context)
    {
        var token = context.Request.Cookies["accessToken"];

        var userSession = _tokenService.GetUserSession(token);

        var appUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userSession.Id);

        if (appUser is null)
        {
            return new ErrorResponse(false, 404, "Usuário não encontrado!");
        }
        
        return new SuccessResponse<ReadUserSessionDTO>(true, 200, "Sessão válida!", userSession);
    }
    
    public void SetTokensInsideCookie(string token, HttpContext context)
    {
        context.Response.Cookies.Append("accessToken", token, new CookieOptions
        {
            Expires = DateTime.Now.AddHours(5),
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
    }
}