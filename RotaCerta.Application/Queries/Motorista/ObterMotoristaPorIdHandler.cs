using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Motorista;

public class ObterMotoristaPorIdHandler : IRequestHandler<ObterMotoristaPorIdQuery, ResultViewModel<Domain.Models.Motorista>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ObterMotoristaPorIdHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<Domain.Models.Motorista>> Handle(
        ObterMotoristaPorIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<Domain.Models.Motorista>.Error("Usuário não autenticado.");

        var motorista = await _unitOfWork.MotoristaRepository
            .GetByIdAsync(motoristaId, cancellationToken);

        if (motorista is null)
            return ResultViewModel<Domain.Models.Motorista>.Error("Motorista não encontrado.");

        return ResultViewModel<Domain.Models.Motorista>.Success(motorista);
    }
}
