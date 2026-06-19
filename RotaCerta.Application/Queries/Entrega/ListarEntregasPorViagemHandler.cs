using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Entrega;

public class ListarEntregasPorViagemHandler : IRequestHandler<ListarEntregasPorViagemQuery, ResultViewModel<List<Domain.Models.Entrega>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ListarEntregasPorViagemHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<List<Domain.Models.Entrega>>> Handle(
        ListarEntregasPorViagemQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<List<Domain.Models.Entrega>>.Error("Usuário não autenticado.");

        var viagem = await _unitOfWork.ViagemRepository
            .GetByIdAsync(request.ViagemId, cancellationToken);

        if (viagem is null)
            return ResultViewModel<List<Domain.Models.Entrega>>.Error("Viagem não encontrada.");

        if (viagem.MotoristaId != motoristaId)
            return ResultViewModel<List<Domain.Models.Entrega>>.Error("Viagem não pertence ao motorista autenticado.");

        var (items, totalPaginas, totalCount) = await _unitOfWork.EntregaRepository
            .GetByViagemIdAsync(request.ViagemId, request.PageNumber, request.PageSize, cancellationToken);

        if (items is null || !items.Any())
            return ResultViewModel<List<Domain.Models.Entrega>>.Error("Nenhuma entrega encontrada.");

        return ResultViewModel<List<Domain.Models.Entrega>>.Success(items, totalPaginas);
    }
}
