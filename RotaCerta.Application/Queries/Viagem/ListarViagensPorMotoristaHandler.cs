using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Viagem;

public class ListarViagensPorMotoristaHandler : IRequestHandler<ListarViagensPorMotoristaQuery, ResultViewModel<List<Domain.Viagens.Viagem>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ListarViagensPorMotoristaHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<List<Domain.Viagens.Viagem>>> Handle(
        ListarViagensPorMotoristaQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<List<Domain.Viagens.Viagem>>.Error("Usuário não autenticado.");

        var (items, totalPaginas, totalCount) = await _unitOfWork.ViagemRepository
            .GetByMotoristaIdAsync(motoristaId, request.Status, request.PageNumber, request.PageSize, cancellationToken);

        if (items is null || !items.Any())
            return ResultViewModel<List<Domain.Viagens.Viagem>>.Error("Nenhuma viagem encontrada.");

        return ResultViewModel<List<Domain.Viagens.Viagem>>.Success(items, totalPaginas);
    }
}
