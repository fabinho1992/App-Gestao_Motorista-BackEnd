using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.VeiculoHandler.Commands.RegistrarTrocaOleo;

public class RegistrarTrocaOleoHandler : IRequestHandler<RegistrarTrocaOleoCommand, ResultViewModel<AlertaOleo>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public RegistrarTrocaOleoHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<AlertaOleo>> Handle(
        RegistrarTrocaOleoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel<AlertaOleo>.Error("Usuário não autenticado.");

            var veiculo = await _unitOfWork.VeiculoRepository
                .GetByIdAsync(request.VeiculoId, cancellationToken);

            if (veiculo is null)
                return ResultViewModel<AlertaOleo>.Error("Veículo não encontrado.");

            if (veiculo.MotoristaId != motoristaId)
                return ResultViewModel<AlertaOleo>.Error("Veículo não pertence ao motorista autenticado.");

            var alerta = veiculo.RegistrarTrocaOleo();

            await _unitOfWork.VeiculoRepository.UpdateAsync(veiculo, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel<AlertaOleo>.Success(alerta);
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel<AlertaOleo>.Error(ex.Message);
        }
    }
}
