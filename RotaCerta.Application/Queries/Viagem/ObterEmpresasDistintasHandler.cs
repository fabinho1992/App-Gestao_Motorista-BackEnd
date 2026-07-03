using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Viagem;

public class ObterEmpresasDistintasHandler : IRequestHandler<ObterEmpresasDistintasQuery, ResultViewModel<List<string>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ObterEmpresasDistintasHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<List<string>>> Handle(
        ObterEmpresasDistintasQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<List<string>>.Error("Usuário não autenticado.");

        var empresas = await _unitOfWork.ViagemRepository
            .GetEmpresasDistintasAsync(motoristaId, cancellationToken);

        return ResultViewModel<List<string>>.Success(empresas);
    }
}
