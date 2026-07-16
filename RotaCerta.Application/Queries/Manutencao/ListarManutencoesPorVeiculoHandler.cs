using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.Queries.Manutencao;

public class ListarManutencoesPorVeiculoHandler : IRequestHandler<ListarManutencoesPorVeiculoQuery, ResultViewModel<List<ManutencaoDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ListarManutencoesPorVeiculoHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<List<ManutencaoDto>>> Handle(
        ListarManutencoesPorVeiculoQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel<List<ManutencaoDto>>.Error("Usuário não autenticado.");

        var veiculo = await _unitOfWork.VeiculoRepository
            .GetByIdAsync(request.VeiculoId, cancellationToken);

        if (veiculo is null)
            return ResultViewModel<List<ManutencaoDto>>.Error("Veículo não encontrado.");

        if (veiculo.MotoristaId != motoristaId)
            return ResultViewModel<List<ManutencaoDto>>.Error("Veículo não pertence ao motorista autenticado.");

        var manutencoes = await _unitOfWork.ManutencaoRepository
            .GetByVeiculoIdAsync(request.VeiculoId, cancellationToken);

        var dtos = manutencoes.Select(m => new ManutencaoDto
        {
            Id = m.Id,
            Tipo = m.Tipo.ToString(),
            Descricao = m.Descricao,
            DataRealizacao = m.DataRealizacao,
            KmRealizacao = m.KmRealizacao,
            Custo = m.Custo,
            Observacao = m.Observacao
        }).ToList();

        // lista vazia também é sucesso — não há critério de alerta que justifique erro
        return ResultViewModel<List<ManutencaoDto>>.Success(dtos);
    }
}
