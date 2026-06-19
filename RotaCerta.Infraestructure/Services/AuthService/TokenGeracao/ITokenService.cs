using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RotaCerta.Infraestructure.Services.AuthService.TokenGeracao;

public interface ITokenService
{
    JwtSecurityToken GenerationToken(IEnumerable<Claim> claims, IConfiguration config);
}
