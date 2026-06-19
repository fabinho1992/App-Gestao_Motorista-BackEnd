using Microsoft.AspNetCore.Identity;
using RotaCerta.Domain.ModelsAuthentication;
using RotaCerta.Domain.Services.IAuthService;
using RotaCerta.Infraestructure.Context.Identity;

namespace RotaCerta.Infraestructure.Services.AuthService;

public class AddRole : IAddRole
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AddRole(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<ResponseIdentityCreate> AdicionarRoles(string userEmail, string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
            return new ResponseIdentityCreate { Status = "404", Message = $"Role '{roleName}' não encontrada." };

        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user is not null)
        {
            var roleAdd = await _userManager.AddToRoleAsync(user, roleName);

            if (roleAdd.Succeeded)
                return new ResponseIdentityCreate { Status = "200", Message = $"Usuário adicionado à role '{roleName}' com sucesso." };
            else
                return new ResponseIdentityCreate { Status = "400", Message = "Erro ao adicionar usuário à role." };
        }

        return new ResponseIdentityCreate { Status = "404", Message = $"Usuário '{userEmail}' não encontrado." };
    }
}
