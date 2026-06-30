using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.Dtos;
using RotaCerta.Application.Queries.Viagem;
using RotaCerta.Application.ViagemHandler.Commands.AbrirViagem;
using RotaCerta.Application.ViagemHandler.Commands.AtualizarStatusPagamento;
using RotaCerta.Application.ViagemHandler.Commands.EncerrarViagem;
using RotaCerta.Domain.Enums;

namespace RotaCerta.API.Controllers;

public record AtualizarStatusPagamentoRequest(StatusPagamento NovoStatus);

/// <summary>
/// Gerencia as viagens do motorista autenticado.
/// </summary>
[ApiController]
[Route("api/v1/viagem")]
[Authorize]
public class ViagemController : ControllerBase
{
    private readonly IMediator _mediator;

    public ViagemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Abre uma nova viagem para o motorista autenticado.</summary>
    /// <response code="201">Viagem aberta com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    [HttpPost]
    [ProducesResponseType(typeof(ResultViewModel<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultViewModel<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Abrir([FromBody] AbrirViagemCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Created(string.Empty, result);
    }

    /// <summary>Encerra uma viagem em andamento.</summary>
    /// <response code="200">Viagem encerrada com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    [HttpPut("{id}/encerrar")]
    [ProducesResponseType(typeof(ResultViewModel<Domain.Models.AlertaOleo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Encerrar(Guid id, [FromBody] EncerrarViagemCommand command)
    {
        var result = await _mediator.Send(command with { ViagemId = id });

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Lista viagens do motorista com filtro opcional por status e paginacao.</summary>
    /// <response code="200">Lista de viagens retornada com sucesso</response>
    /// <response code="400">Nenhuma viagem encontrada</response>
    /// <response code="401">Nao autorizado</response>
    [HttpGet]
    [ProducesResponseType(typeof(ResultViewModel<List<Domain.Viagens.Viagem>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Listar(
        [FromQuery] StatusViagem? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new ListarViagensPorMotoristaQuery
        {
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>Atualiza o status de pagamento de uma viagem.</summary>
    /// <response code="200">Status atualizado com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    [HttpPut("{id}/status-pagamento")]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AtualizarStatusPagamento(
        Guid id,
        [FromBody] AtualizarStatusPagamentoRequest request,
        CancellationToken ct)
    {
        var command = new AtualizarStatusPagamentoCommand(id, request.NovoStatus);
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>Obtem uma viagem pelo ID.</summary>
    /// <response code="200">Viagem retornada com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    /// <response code="404">Viagem nao encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResultViewModel<Domain.Viagens.Viagem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var result = await _mediator.Send(new ObterViagemPorIdQuery(id));

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}
