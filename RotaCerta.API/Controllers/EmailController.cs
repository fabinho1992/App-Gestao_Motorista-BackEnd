using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.EmailHandler.Commands.ChangePassword;
using RotaCerta.Application.EmailHandler.Commands.ConfirmacaoEmail;
using RotaCerta.Application.EmailHandler.Commands.ConfirmarEmail;
using RotaCerta.Application.EmailHandler.Commands.ConsultaStatusEmail;
using RotaCerta.Application.EmailHandler.Commands.ResetPassword;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services.IEmail;

namespace RotaCerta.API.Controllers
{
    [Route("api/v1/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("solicitar-reset")]
        public async Task<IActionResult> SolicitarReset([FromBody] ResetCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("alterar")]
        public async Task<IActionResult> Alterar([FromBody] ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("status")]
        public async Task<IActionResult> Status()
        {
            var result = await _mediator.Send(new ConsultarStatusEmailQuery());
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("solicitar-confirmacao")]
        public async Task<IActionResult> SolicitarConfirmacao()
        {
            var result = await _mediator.Send(new SolicitarConfirmacaoEmailCommand());
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("confirmar")]
        public async Task<IActionResult> Confirmar([FromBody] ConfirmarEmailCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}
