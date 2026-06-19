using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Veiculo;

public class ListarVeiculosPorMotoristaHandler : IRequestHandler<ListarVeiculosPorMotoristaQuery, ResultViewModel<List<Domain.Models.Veiculo>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ListarVeiculosPorMotoristaHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<List<Domain.Models.Veiculo>>> Handle(
        ListarVeiculosPorMotoristaQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<List<Domain.Models.Veiculo>>.Error("Usuário não autenticado.");

        var (items, totalPaginas, totalCount) = await _unitOfWork.VeiculoRepository
            .GetByMotoristaIdAsync(motoristaId, request.PageNumber, request.PageSize, cancellationToken);

        if (items is null || !items.Any())
            return ResultViewModel<List<Domain.Models.Veiculo>>.Error("Nenhum veículo encontrado.");

        return ResultViewModel<List<Domain.Models.Veiculo>>.Success(items, totalPaginas);
    }
}
