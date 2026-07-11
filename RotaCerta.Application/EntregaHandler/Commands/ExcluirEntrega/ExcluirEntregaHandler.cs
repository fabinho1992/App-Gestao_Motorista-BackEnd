using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.EntregaHandler.Commands.ExcluirEntrega;

public class ExcluirEntregaHandler : IRequestHandler<ExcluirEntregaCommand, ResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ExcluirEntregaHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel> Handle(
        ExcluirEntregaCommand request,
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

            if (viagem is null)
                return ResultViewModel.Error("Viagem não encontrada.");

            if (viagem.MotoristaId != motoristaId)
                return ResultViewModel.Error("Entrega não pertence ao motorista autenticado.");

            if (entrega.Status != StatusEntrega.Pendente)
                return ResultViewModel.Error(
                    "Não é possível excluir uma entrega que já foi confirmada ou falhou. " +
                    "Somente entregas pendentes podem ser excluídas.");

            entrega.MarcarComoExcluido();

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
