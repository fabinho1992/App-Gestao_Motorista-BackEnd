using RotaCerta.Domain.ModelsAuthentication;

namespace RotaCerta.Domain.Services.IAuthService;

public interface ILoginUser
{
    Task<ResponseLogin> LoginAsync(Login login);
}
