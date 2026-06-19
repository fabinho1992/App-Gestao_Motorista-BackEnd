using Microsoft.AspNetCore.Http;
using RotaCerta.Domain.Services;
using System.Security.Claims;

namespace RotaCerta.Infraestructure.Services.AuthService;

public class UsuarioContext : IUsuarioContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Email =>
        _httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public string MotoristaId =>
        _httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
}
