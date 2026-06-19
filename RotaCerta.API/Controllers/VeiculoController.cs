using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.Dtos;
using RotaCerta.Application.MotoristaHandler.Commands.CriarVeiculo;
using RotaCerta.Application.Queries.Veiculo;
using RotaCerta.Application.VeiculoHandler.Commands.ExcluirVeiculo;

namespace RotaCerta.API.Controllers;

/// <summary>
/// Gerencia os veiculos do motorista autenticado.
/// </summary>
[ApiController]
[Route("api/v1/veiculo")]
[Authorize]
public class VeiculoController : ControllerBase
{
    private readonly IMediator _mediator;

    public VeiculoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Cadastra um novo veiculo para o motorista autenticado.</summary>
    /// <response code="201">Veiculo criado com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    [HttpPost]
    [ProducesResponseType(typeof(ResultViewModel<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultViewModel<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarVeiculoCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Created(string.Empty, result);
    }

    /// <summary>Lista todos os veiculos do motorista autenticado com paginacao.</summary>
    /// <response code="200">Lista de veiculos retornada com sucesso</response>
    /// <response code="400">Nenhum veiculo encontrado</response>
    /// <response code="401">Nao autorizado</response>
    [HttpGet]
    [ProducesResponseType(typeof(ResultViewModel<List<Domain.Models.Veiculo>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new ListarVeiculosPorMotoristaQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>Obtem um veiculo pelo ID com alerta de oleo calculado.</summary>
    /// <response code="200">Veiculo retornado com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    /// <response code="404">Veiculo nao encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResultViewModel<VeiculoComAlertaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var result = await _mediator.Send(new ObterVeiculoPorIdQuery(id));

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var command = new ExcluirVeiculoCommand(id);
        var result = await _mediator.Send(command, ct);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
