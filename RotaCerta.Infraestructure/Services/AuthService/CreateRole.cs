using Microsoft.AspNetCore.Identity;
using RotaCerta.Domain.ModelsAuthentication;
using RotaCerta.Domain.Services.IAuthService;
using RotaCerta.Infraestructure.Context.Identity;

namespace RotaCerta.Infraestructure.Services.AuthService;

public class CreateRole : ICreateRole
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateRole(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<ResponseIdentityCreate> CreateRoleAsync(string roleName)
    {
        var roleExiste = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExiste)
        {
            var role = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (role.Succeeded)
                return new ResponseIdentityCreate { Message = $"Role '{roleName}' criada com sucesso!", Status = "200" };
            else
                return new ResponseIdentityCreate { Status = "400", Message = "Erro ao criar role." };
        }

        return new ResponseIdentityCreate { Status = "400", Message = "Role já existe." };
    }
}
