using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.EntregaHandler.Commands.RegistrarFalhaEntrega;

public class RegistrarFalhaEntregaHandler : IRequestHandler<RegistrarFalhaEntregaCommand, ResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public RegistrarFalhaEntregaHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel> Handle(
        RegistrarFalhaEntregaCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel.Error("Usuário não autenticado.");

            var entrega = await _unitOfWork.EntregaRepository
                .GetByIdAsync(request.EntregaId, cancellationToken);

            if (entrega is null)
                return ResultViewModel.Error("Entrega não encontrada.");

            var viagem = await _unitOfWork.ViagemRepository
                .GetByIdAsync(entrega.ViagemId, cancellationToken);

            if (viagem is null || viagem.MotoristaId != motoristaId)
                return ResultViewModel.Error("Entrega não pertence ao motorista autenticado.");

            // levanta EntregaFalhadaEvent
            entrega.RegistrarFalha(request.Motivo);

            await _unitOfWork.EntregaRepository.UpdateAsync(entrega, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel.Success();
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel.Error(ex.Message);
        }
    }
}
