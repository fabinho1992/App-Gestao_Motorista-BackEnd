using RotaCerta.Domain.ModelsAuthentication;

namespace RotaCerta.Domain.Services.IAuthService;

public interface ICreateRole
{
    Task<ResponseIdentityCreate> CreateRoleAsync(string roleName);
}
