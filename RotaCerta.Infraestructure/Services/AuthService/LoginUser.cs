using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RotaCerta.Domain.ModelsAuthentication;
using RotaCerta.Domain.Services.IAuthService;
using RotaCerta.Infraestructure.Context.Identity;
using RotaCerta.Infraestructure.Services.AuthService.TokenGeracao;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RotaCerta.Infraestructure.Services.AuthService;

public class LoginUser : ILoginUser
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public LoginUser(
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<ResponseLogin> LoginAsync(Login login)
    {
        var usuario = await _userManager.FindByEmailAsync(login.Email!);

        if (usuario is not null && await _userManager.CheckPasswordAsync(usuario, login.Senha!))
        {
            var usuarioRoles = await _userManager.GetRolesAsync(usuario);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.UserName!),
                new Claim(ClaimTypes.Email, usuario.Email!),
                new Claim("id", usuario.UserName!),
                new Claim(ClaimTypes.NameIdentifier, usuario.MotoristaId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var usuarioRole in usuarioRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, usuarioRole));

            var token = _tokenService.GenerationToken(authClaims, _configuration);

            await _userManager.UpdateAsync(usuario);

            return new ResponseLogin
            {
                DisplayName = usuario.DisplayName,
                MotoristaId = usuario.MotoristaId,  // ← adiciona essa linha
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expired = token.ValidTo,
                Status = "Sucess 200",
                Message = "Token gerado com sucesso."
            };
        }

        return new ResponseLogin
        {
            Status = "Bad Request 400",
            Message = "Email ou senha inválidos."
        };
    }
}
