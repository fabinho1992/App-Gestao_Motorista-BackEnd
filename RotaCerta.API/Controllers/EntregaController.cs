using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.Dtos;
using RotaCerta.Application.EntregaHandler.Commands.AdicionarEntrega;
using RotaCerta.Application.EntregaHandler.Commands.ConfirmarEntrega;
using RotaCerta.Application.EntregaHandler.Commands.RegistrarFalhaEntrega;
using RotaCerta.Application.Queries.Entrega;

namespace RotaCerta.API.Controllers;

/// <summary>
/// Gerencia as entregas vinculadas a uma viagem.
/// </summary>
[ApiController]
[Route("api/v1/entrega")]
[Authorize]
public class EntregaController : ControllerBase
{
    private readonly IMediator _mediator;

    public EntregaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Adiciona uma nova entrega a uma viagem.</summary>
    /// <response code="201">Entrega adicionada com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    [HttpPost]
    [ProducesResponseType(typeof(ResultViewModel<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultViewModel<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Adicionar([FromBody] AdicionarEntregaCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Created(string.Empty, result);
    }

    /// <summary>Confirma a entrega com upload de fotos comprovantes.</summary>
    /// <response code="200">Entrega confirmada com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    [HttpPut("{id}/confirmar")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirmar(Guid id, [FromForm] List<IFormFile>? fotos)
    {
        var command = new ConfirmarEntregaCommand(id, fotos);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Registra uma falha na entrega com o motivo.</summary>
    /// <response code="200">Falha registrada com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    [HttpPut("{id}/falha")]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarFalha(Guid id, [FromBody] RegistrarFalhaEntregaCommand command)
    {
        var result = await _mediator.Send(command with { EntregaId = id });

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Lista todas as entregas de uma viagem.</summary>
    /// <response code="200">Lista de entregas retornada com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    [HttpGet("viagem/{viagemId}")]
    [ProducesResponseType(typeof(ResultViewModel<List<Domain.Models.Entrega>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarPorViagem(Guid viagemId)
    {
        var result = await _mediator.Send(new ListarEntregasPorViagemQuery(viagemId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
