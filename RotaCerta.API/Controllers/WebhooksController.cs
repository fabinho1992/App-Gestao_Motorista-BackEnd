using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.Assinaturas.Command.ProcessarPagamento;
using RotaCerta.Domain.Services.Pagamento.Dtos;
using System.Security.Cryptography;
using System.Text;

namespace RotaCerta.API.Controllers
{
    [ApiController]
    [Route("api/v1/webhooks")]
    [AllowAnonymous]
    public class WebhooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public WebhooksController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpPost("pagamento")]
        public async Task<IActionResult> Pagamento([FromBody] AsaasWebhookPayload payload, CancellationToken cancellationToken)
        {
            var tokenRecebido = Request.Headers["asaas-access-token"].FirstOrDefault();
            var tokenEsperado = _configuration["Asaas:WebhookToken"];

            if (string.IsNullOrEmpty(tokenEsperado) || string.IsNullOrEmpty(tokenRecebido) ||
                !CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(tokenRecebido),
                    Encoding.UTF8.GetBytes(tokenEsperado)))
            {
                return Unauthorized();
            }

            var command = new ProcessarWebhookPagamentoCommand(
                payload.Event,
                payload.Payment?.Subscription,
                payload.Payment?.Id,
                payload.Payment?.Status);

            await _mediator.Send(command, cancellationToken);

            return Ok(new { received = true });
        }
    }
}
