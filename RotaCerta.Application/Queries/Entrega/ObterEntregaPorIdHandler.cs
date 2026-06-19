using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Entrega;

public class ObterEntregaPorIdHandler : IRequestHandler<ObterEntregaPorIdQuery, ResultViewModel<Domain.Models.Entrega>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ObterEntregaPorIdHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<Domain.Models.Entrega>> Handle(
        ObterEntregaPorIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<Domain.Models.Entrega>.Error("Usuário não autenticado.");

        var entrega = await _unitOfWork.EntregaRepository
            .GetByIdAsync(request.EntregaId, cancellationToken);

        if (entrega is null)
            return ResultViewModel<Domain.Models.Entrega>.Error("Entrega não encontrada.");

        // verifica propriedade via viagem
        var viagem = await _unitOfWork.ViagemRepository
            .GetByIdAsync(entrega.ViagemId, cancellationToken);

        if (viagem is null || viagem.MotoristaId != motoristaId)
            return ResultViewModel<Domain.Models.Entrega>.Error("Entrega não pertence ao motorista autenticado.");

        return ResultViewModel<Domain.Models.Entrega>.Success(entrega);
    }
}
