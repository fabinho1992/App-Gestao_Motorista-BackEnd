using RotaCerta.Domain.ModelsAuthentication;

namespace RotaCerta.Domain.Services.IAuthService;

public interface ICreateUser
{
    Task<ResponseIdentityCreate> CreateUserAsync(RegisterUser registerUser);
}
