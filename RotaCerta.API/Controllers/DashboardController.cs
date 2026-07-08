using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.Dtos;
using RotaCerta.Application.Queries.Dashboard;

namespace RotaCerta.API.Controllers;

/// <summary>
/// Resumo financeiro e operacional do motorista.
/// </summary>
[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Retorna o resumo mensal do motorista autenticado.</summary>
    /// <response code="200">Resumo retornado com sucesso</response>
    /// <response code="400">Erro de validacao</response>
    /// <response code="401">Nao autorizado</response>
    [HttpGet("resumo")]
    [ProducesResponseType(typeof(ResultViewModel<ResumoDashboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Resumo(
        [FromQuery] int mes = 0,
        [FromQuery] int ano = 0,
        CancellationToken ct = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var query = new ObterResumoDashboardQuery(
            mes == 0 ? hoje.Month : mes,
            ano == 0 ? hoje.Year : ano);

        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>Retorna o relatório mensal de gastos de combustível.</summary>
    /// <response code="200">Relatório retornado com sucesso</response>
    /// <response code="400">Erro de validação</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("relatorio-combustivel")]
    [ProducesResponseType(typeof(ResultViewModel<RelatorioCombustivelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RelatorioCombustivel(
        [FromQuery] int mes = 0,
        [FromQuery] int ano = 0,
        CancellationToken ct = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var query = new ObterRelatorioCombustivelQuery(
            mes == 0 ? hoje.Month : mes,
            ano == 0 ? hoje.Year : ano);

        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
