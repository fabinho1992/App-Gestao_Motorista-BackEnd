using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.ViagemHandler.Commands.ExcluirViagem;

public class ExcluirViagemHandler : IRequestHandler<ExcluirViagemCommand, ResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ExcluirViagemHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel> Handle(
        ExcluirViagemCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel.Error("Usuário não autenticado.");

            // GetByIdAsync já carrega Entregas via Include
            var viagem = await _unitOfWork.ViagemRepository
                .GetByIdAsync(request.ViagemId, cancellationToken);

            if (viagem is null)
                return ResultViewModel.Error("Viagem não encontrada.");

            if (viagem.MotoristaId != motoristaId)
                return ResultViewModel.Error("Viagem não pertence ao motorista autenticado.");

            if (viagem.Status != StatusViagem.Aberta &&
                viagem.Status != StatusViagem.EmRota)
                return ResultViewModel.Error(
                    "Não é possível excluir uma viagem encerrada.");

            foreach (var entrega in viagem.Entregas)
            {
                entrega.MarcarComoExcluido();
                await _unitOfWork.EntregaRepository.UpdateAsync(entrega);
            }

            viagem.MarcarComoExcluido();

            await _unitOfWork.ViagemRepository.UpdateAsync(viagem, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel.Success();
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel.Error(ex.Message);
        }
    }
}
