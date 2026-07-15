using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.ViagemHandler.Commands.EncerrarViagem;

public class EncerrarViagemHandler : IRequestHandler<EncerrarViagemCommand, ResultViewModel<AlertaOleo>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public EncerrarViagemHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<AlertaOleo>> Handle(
        EncerrarViagemCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel<AlertaOleo>.Error("Usuário não autenticado.");

            // GetByIdAsync já carrega Entregas e Veiculo via Include
            var viagem = await _unitOfWork.ViagemRepository
                .GetByIdAsync(request.ViagemId, cancellationToken);

            if (viagem is null)
                return ResultViewModel<AlertaOleo>.Error("Viagem não encontrada.");

            if (viagem.MotoristaId != motoristaId)
                return ResultViewModel<AlertaOleo>.Error("Viagem não pertence ao motorista autenticado.");

            if (request.KmFinal < 0)
                return ResultViewModel<AlertaOleo>.Error(
                    "Km final não pode ser negativo.");

            if (request.KmFinal < viagem.KmInicial)
                return ResultViewModel<AlertaOleo>.Error(
                    $"Km final não pode ser menor que o km inicial da viagem ({viagem.KmInicial:N0} km).");

            if (request.GastoCombustivel < 0 ||
                request.GastoPedagio     < 0 ||
                request.GastoAlimentacao < 0 ||
                request.GastoOutros      < 0)
                return ResultViewModel<AlertaOleo>.Error(
                    "Os valores de gastos não podem ser negativos.");

            if (request.PrecoCombustivelLitro < 0)
                return ResultViewModel<AlertaOleo>.Error(
                    "Preço do combustível por litro não pode ser negativo.");

            // Encerrar: atualiza km do veículo e levanta KmAtualizadoEvent + ViagemEncerradaEvent
            var alerta = viagem.Encerrar(
                request.KmFinal,
                request.GastoCombustivel,
                request.GastoPedagio,
                request.GastoAlimentacao,
                request.GastoOutros,
                request.ObsEncerramento,
                request.PrecoCombustivelLitro);

            await _unitOfWork.ViagemRepository.UpdateAsync(viagem, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel<AlertaOleo>.Success(alerta);
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel<AlertaOleo>.Error(ex.Message);
        }
    }
}
