using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.Dtos;
using RotaCerta.Application.ManutencaoHandler.Commands.ExcluirManutencao;
using RotaCerta.Application.ManutencaoHandler.Commands.RegistrarManutencao;
using RotaCerta.Application.Queries.Manutencao;

namespace RotaCerta.API.Controllers;

/// <summary>
/// Gerencia as manutenções dos veículos do motorista autenticado.
/// </summary>
[ApiController]
[Route("api/v1/manutencao")]
[Authorize]
public class ManutencaoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ManutencaoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Registra uma nova manutenção para um veículo do motorista autenticado.</summary>
    /// <response code="201">Manutenção registrada com sucesso</response>
    /// <response code="400">Erro de validação</response>
    /// <response code="401">Não autorizado</response>
    [HttpPost]
    [ProducesResponseType(typeof(ResultViewModel<ManutencaoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultViewModel<ManutencaoDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarManutencaoCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Created(string.Empty, result);
    }

    /// <summary>Lista as manutenções de um veículo do motorista autenticado.</summary>
    /// <response code="200">Lista de manutenções retornada com sucesso</response>
    /// <response code="400">Erro de validação</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("veiculo/{veiculoId}")]
    [ProducesResponseType(typeof(ResultViewModel<List<ManutencaoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarPorVeiculo(Guid veiculoId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListarManutencoesPorVeiculoQuery(veiculoId), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>Exclui (soft delete) uma manutenção do motorista autenticado.</summary>
    /// <response code="200">Manutenção excluída com sucesso</response>
    /// <response code="400">Erro de validação</response>
    /// <response code="401">Não autorizado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var command = new ExcluirManutencaoCommand(id);
        var result = await _mediator.Send(command, ct);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
