using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Dashboard;

public class ObterResumoDashboardHandler : IRequestHandler<ObterResumoDashboardQuery, ResultViewModel<ResumoDashboardDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ObterResumoDashboardHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<ResumoDashboardDto>> Handle(
    ObterResumoDashboardQuery request,
    CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<ResumoDashboardDto>.Error("Usuário não autenticado.");

        var viagens = await _unitOfWork.ViagemRepository
            .GetByMotoristaIdMesAsync(motoristaId, request.Mes, request.Ano, cancellationToken);

        // ← filtra só encerradas e pagas para os cálculos financeiros
        var viagensContabilizadas = viagens
            .Where(v => v.Status == StatusViagem.Encerrada &&
                        v.StatusPagamento == StatusPagamento.Pago)
            .ToList();

        var viagemAtiva = viagens.FirstOrDefault(v =>
            v.Status == StatusViagem.Aberta ||
            v.Status == StatusViagem.EmRota);

        var dto = new ResumoDashboardDto
        {
            // ← usa viagensContabilizadas para valores financeiros
            TotalGanhosMes = viagensContabilizadas.Sum(v => v.ValorFrete),
            TotalGastosMes = viagensContabilizadas.Sum(v => v.TotalGastos),
            LucroLiquidoMes = viagensContabilizadas.Sum(v => v.SaldoLiquido),
            TotalKmRodadosMes = viagensContabilizadas
                .Where(v => v.KmRodado.HasValue)
                .Sum(v => v.KmRodado!.Value),

            // ← usa viagens completas para contagens gerais
            TotalViagensMes = viagens.Count,
            TotalEntregasMes = viagens.Sum(v => v.Entregas.Count),

            // ← viagem ativa continua usando todas as viagens
            TemViagemAtiva = viagemAtiva is not null,
            ViagemAtivaId = viagemAtiva?.Id,
            ViagemAtivaEmpresa = viagemAtiva?.EmpresaContratante,
            ViagemAtivaStatus = viagemAtiva?.Status.ToString(),
            EntregasConcluidasAtiva = viagemAtiva?.TotalEntregasRealizadas ?? 0,
            TotalEntregasAtiva = viagemAtiva?.Entregas.Count ?? 0
        };

        return ResultViewModel<ResumoDashboardDto>.Success(dto);
    }
}
