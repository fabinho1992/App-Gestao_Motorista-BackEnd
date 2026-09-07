using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.EmailHandler.Commands.ChangePassword;
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
    }
    }
