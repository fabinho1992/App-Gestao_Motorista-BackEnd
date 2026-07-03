using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.Dtos;
using RotaCerta.Application.MotoristaHandler.Commands.AtualizarMotorista;
using RotaCerta.Application.MotoristaHandler.Commands.ResetarDados;
using RotaCerta.Application.Queries.Motorista;
using RotaCerta.Application.Queries.Veiculo;

namespace RotaCerta.API.Controllers;

[ApiController]
[Route("api/v1/motorista")]
[Authorize]
public class MotoristaController : ControllerBase
{
    private readonly IMediator _mediator;

    public MotoristaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("perfil")]
    [ProducesResponseType(typeof(ResultViewModel<VeiculoComAlertaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var result = await _mediator.Send(new ObterMotoristaPorIdQuery());

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut]
    [Authorize]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Atualizar([FromBody] AtualizarMotoristaCommand command)
    {
        var result = await _mediator.Send(command);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>Reseta todos os dados do motorista autenticado.</summary>
    /// <response code="200">Dados resetados com sucesso</response>
    /// <response code="400">Erro ao resetar</response>
    /// <response code="401">Nao autorizado</response>
    [HttpDelete("resetar")]
    [Authorize]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Resetar(CancellationToken ct)
    {
        var result = await _mediator.Send(new ResetarDadosCommand(), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
