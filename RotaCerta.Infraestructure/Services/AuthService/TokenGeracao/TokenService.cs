using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RotaCerta.Infraestructure.Services.AuthService.TokenGeracao;

public class TokenService : ITokenService
{
    public JwtSecurityToken GenerationToken(IEnumerable<Claim> claims, IConfiguration config)
    {
        var keyString = config.GetSection("Jwt").GetValue<string>("SecretKey")
            ?? throw new InvalidOperationException("SecretKey JWT não configurada.");

        var key = Encoding.UTF8.GetBytes(keyString);

        var assinaturaCredencial = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var descricaoToken = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(config.GetSection("Jwt").GetValue<double>("TokenValidityInMinutes")),
            Issuer = config.GetSection("Jwt").GetValue<string>("ValidIssuer"),
            Audience = config.GetSection("Jwt").GetValue<string>("ValidAudience"),
            SigningCredentials = assinaturaCredencial
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.CreateJwtSecurityToken(descricaoToken);
    }
}
