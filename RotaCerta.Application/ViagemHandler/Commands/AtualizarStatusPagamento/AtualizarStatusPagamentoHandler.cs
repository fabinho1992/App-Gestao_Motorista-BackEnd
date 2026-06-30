using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.ViagemHandler.Commands.AtualizarStatusPagamento;

public class AtualizarStatusPagamentoHandler : IRequestHandler<AtualizarStatusPagamentoCommand, ResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public AtualizarStatusPagamentoHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel> Handle(
        AtualizarStatusPagamentoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel.Error("Usuário não autenticado.");

            var viagem = await _unitOfWork.ViagemRepository
                .GetByIdAsync(request.ViagemId, cancellationToken);

            if (viagem is null)
                return ResultViewModel.Error("Viagem não encontrada.");

            if (viagem.MotoristaId != motoristaId)
                return ResultViewModel.Error("Viagem não pertence ao motorista autenticado.");

            viagem.AtualizarStatusPagamento(request.NovoStatus);

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
