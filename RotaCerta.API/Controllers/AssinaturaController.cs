using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.Assinaturas.Command;
using RotaCerta.Application.Assinaturas.Command.IniciarAssinatura;
using RotaCerta.Application.Assinaturas.Command.RenovarAssinatura;
using RotaCerta.Application.Queries.Assinatura;

namespace RotaCerta.API.Controllers
{
    [Route("api/v1/assinatura")]
    [ApiController]
    public class AssinaturaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssinaturaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var result = await _mediator.Send(new IniciarAssinaturaCommand());
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("renovar")]
        public async Task<IActionResult> Renovar()
        {
            var result = await _mediator.Send(new RenovarAssinaturaCommand());
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("status")]
        public async Task<IActionResult> Status()
        {
            var result = await _mediator.Send(new ConsultarAssinaturaQuery());
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}
