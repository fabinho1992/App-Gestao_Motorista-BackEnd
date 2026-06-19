using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Veiculo;

public class ObterVeiculoPorIdHandler : IRequestHandler<ObterVeiculoPorIdQuery, ResultViewModel<VeiculoComAlertaDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ObterVeiculoPorIdHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<VeiculoComAlertaDto>> Handle(
        ObterVeiculoPorIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<VeiculoComAlertaDto>.Error("Usuário não autenticado.");

        var veiculo = await _unitOfWork.VeiculoRepository
            .GetByIdAsync(request.VeiculoId, cancellationToken);

        if (veiculo is null)
            return ResultViewModel<VeiculoComAlertaDto>.Error("Veículo não encontrado.");

        if (veiculo.MotoristaId != motoristaId)
            return ResultViewModel<VeiculoComAlertaDto>.Error("Veículo não pertence ao motorista autenticado.");

        var alerta = veiculo.VerificarAlertaOleo();

        var dto = new VeiculoComAlertaDto
        {
            Id = veiculo.Id,
            Placa = veiculo.Placa,
            Modelo = veiculo.Modelo,
            Ano = veiculo.Ano,
            TipoCombustivel = veiculo.TipoCombustivel.ToString(),
            KmAtual = veiculo.KmAtual,
            KmUltimoOleo = veiculo.KmUltimoOleo,
            DataUltimoOleo = veiculo.DataUltimoOleo,
            IntervaloOleo = veiculo.IntervaloOleo,
            AlertaOleo = new AlertaOleoDto
            {
                Nivel = alerta.Nivel.ToString(),
                Mensagem = alerta.Mensagem,
                KmFaltando = alerta.KmFaltando
            },
            // ← monta a lista de viagens
            Viagens = veiculo.Viagens.Select(v => new ViagemResumoDto
            {
                Id = v.Id,
                EmpresaContratante = v.EmpresaContratante,
                DataSaida = v.DataSaida,
                Status = v.Status.ToString(),
                ValorFrete = v.ValorFrete,
                SaldoLiquido = v.SaldoLiquido,
                KmRodado = v.KmRodado,
                TotalEntregas = v.Entregas.Count
            }).OrderByDescending(v => v.DataSaida).ToList()
        };

        return ResultViewModel<VeiculoComAlertaDto>.Success(dto);
    }
}
