using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Viagem;

public class ObterViagemPorIdHandler : IRequestHandler<ObterViagemPorIdQuery, ResultViewModel<ViagemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ObterViagemPorIdHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<ViagemDto>> Handle(
        ObterViagemPorIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<ViagemDto>.Error("Usuário não autenticado.");

        var viagem = await _unitOfWork.ViagemRepository
            .GetByIdAsync(request.ViagemId, cancellationToken);

        if (viagem is null)
            return ResultViewModel<ViagemDto>.Error("Viagem não encontrada.");

        if (viagem.MotoristaId != motoristaId)
            return ResultViewModel<ViagemDto>.Error("Viagem não pertence ao motorista autenticado.");

        // mapeia a entidade para o DTO antes de retornar
        var dto = new ViagemDto
        {
            Id = viagem.Id,
            EmpresaContratante = viagem.EmpresaContratante,
            Origem = viagem.Origem,
            DataSaida = viagem.DataSaida,
            KmInicial = viagem.KmInicial,
            KmFinal = viagem.KmFinal,
            ValorFrete = viagem.ValorFrete,
            FormaPagamento = viagem.FormaPagamento.ToString(),
            Pago = viagem.Pago,
            Status = viagem.Status.ToString(),
            GastoCombustivel = viagem.GastoCombustivel,
            GastoPedagio = viagem.GastoPedagio,
            GastoAlimentacao = viagem.GastoAlimentacao,
            GastoOutros = viagem.GastoOutros,
            ObsEncerramento = viagem.ObsEncerramento,
            TotalGastos = viagem.TotalGastos,
            SaldoLiquido = viagem.SaldoLiquido,
            KmRodado = viagem.KmRodado,
            TotalEntregasRealizadas = viagem.TotalEntregasRealizadas,

            Veiculo = viagem.Veiculo is null ? null : new VeiculoResumoDto
            {
                Id = viagem.Veiculo.Id,
                Placa = viagem.Veiculo.Placa,
                Modelo = viagem.Veiculo.Modelo
            },

            Entregas = viagem.Entregas.Select(e => new EntregaDto
            {
                Id = e.Id,
                ViagemId = e.ViagemId,
                Cliente = e.Cliente,
                EnderecoDestino = e.EnderecoDestino,
                Status = e.Status.ToString(),
                Observacao = e.Observacao,
                DataHoraEntrega = e.DataHoraEntrega,
                Fotos = e.Fotos
            }).ToList()
        };

        return ResultViewModel<ViagemDto>.Success(dto);
    }
}
