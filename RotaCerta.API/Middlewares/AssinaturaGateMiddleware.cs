using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.API.Middlewares
{
    public class AssinaturaGateMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly string[] RotasLiberadas =
        {
        "/api/v1/auth",
        "/api/v1/webhooks",
        "/api/v1/motorista/perfil",
        "/api/v1/assinatura"
    };

        public AssinaturaGateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
        {
            var path = context.Request.Path.Value ?? "";

            if (RotasLiberadas.Any(r => path.StartsWith(r, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            if (!Guid.TryParse(usuarioContext.MotoristaId, out var motoristaId))
            {
                // sem token válido ainda — quem barra isso é o [Authorize], não esse middleware
                await _next(context);
                return;
            }

            var assinatura = await unitOfWork.AssinaturaRepository
                .GetByMotoristaIdAsync(motoristaId, context.RequestAborted);

            if (assinatura is null || !assinatura.TemAcessoLiberado())
            {
                context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    data = (object?)null,
                    isSuccess = false,
                    message = "Seu período de teste acabou. Assine o RotaCerta para continuar usando."
                });
                return;
            }

            await _next(context);
        }
    }
}
