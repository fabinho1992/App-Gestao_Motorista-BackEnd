using Microsoft.AspNetCore.Identity;
using RotaCerta.Domain.ModelsAuthentication;
using RotaCerta.Domain.Services.IAuthService;
using RotaCerta.Infraestructure.Context.Identity;

namespace RotaCerta.Infraestructure.Services.AuthService;

public class CreateUser : ICreateUser
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUser(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ResponseIdentityCreate> CreateUserAsync(RegisterUser registerUser)
    {
        var usuarioExiste = await _userManager.FindByEmailAsync(registerUser.UserName);

        if (usuarioExiste != null)
            return new ResponseIdentityCreate { Status = "Erro", Message = "Usuário já existe!" };

        ApplicationUser user = new()
        {
            DisplayName = registerUser.UserName,
            UserName = registerUser.Email,
            Email = registerUser.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            MotoristaId = registerUser.UsuarioId
        };

        var resultado = await _userManager.CreateAsync(user, registerUser.Password!);

        if (!resultado.Succeeded)
            return new ResponseIdentityCreate { Status = "Erro", Message = "Erro ao criar usuário." };

        return new ResponseIdentityCreate { Status = "Ok", Message = "Usuário criado com sucesso." };
    }
}
