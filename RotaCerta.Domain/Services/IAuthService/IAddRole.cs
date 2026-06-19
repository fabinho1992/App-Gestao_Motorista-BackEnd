using RotaCerta.Domain.ModelsAuthentication;

namespace RotaCerta.Domain.Services.IAuthService;

public interface IAddRole
{
    Task<ResponseIdentityCreate> AdicionarRoles(string userEmail, string roleName);
}
