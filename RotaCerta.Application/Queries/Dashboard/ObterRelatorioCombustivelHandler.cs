using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Dashboard;

public class ObterRelatorioCombustivelHandler : IRequestHandler<ObterRelatorioCombustivelQuery, ResultViewModel<RelatorioCombustivelDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    private static readonly string[] Meses =
    [
        "Janeiro", "Fevereiro", "Março", "Abril",
        "Maio", "Junho", "Julho", "Agosto",
        "Setembro", "Outubro", "Novembro", "Dezembro"
    ];

    public ObterRelatorioCombustivelHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<RelatorioCombustivelDto>> Handle(
        ObterRelatorioCombustivelQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<RelatorioCombustivelDto>.Error("Usuário não autenticado.");

        var viagens = await _unitOfWork.ViagemRepository
            .GetViagensEncerradasPorMesAsync(motoristaId, request.Mes, request.Ano, cancellationToken);

        var dto = new RelatorioCombustivelDto
        {
            Mes = request.Mes,
            Ano = request.Ano,
            NomeMes = Meses[request.Mes - 1],
            TotalGastoCombustivel = viagens.Sum(v => v.GastoCombustivel),
            TotalGastoPedagio = viagens.Sum(v => v.GastoPedagio),
            TotalGastoAlimentacao = viagens.Sum(v => v.GastoAlimentacao),
            TotalGastoOutros = viagens.Sum(v => v.GastoOutros),
            TotalKmRodado = viagens
                .Where(v => v.KmRodado.HasValue)
                .Sum(v => v.KmRodado!.Value),
            TotalViagensEncerradas = viagens.Count,
            Viagens = viagens.Select(v => new DetalheCombustivelDto
            {
                ViagemId = v.Id,
                EmpresaContratante = v.EmpresaContratante,
                DataEncerramento = v.DataFim?.ToString("yyyy-MM-dd") ?? string.Empty,
                GastoCombustivel = v.GastoCombustivel,
                GastoPedagio = v.GastoPedagio,
                GastoAlimentacao = v.GastoAlimentacao,
                GastoOutros = v.GastoOutros,
                TotalGastos = v.TotalGastos,
                KmRodado = v.KmRodado ?? 0,
                ValorFrete = v.ValorFrete,
                SaldoLiquido = v.SaldoLiquido
            }).ToList()
        };

        dto.TotalGastosGeral = dto.TotalGastoCombustivel + dto.TotalGastoPedagio +
            dto.TotalGastoAlimentacao + dto.TotalGastoOutros;

        dto.MediaKmPorLitro = dto.TotalGastoCombustivel > 0
            ? dto.TotalKmRodado / dto.TotalGastoCombustivel
            : 0;

        return ResultViewModel<RelatorioCombustivelDto>.Success(dto);
    }
}
