using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.Queries.Manutencao;

public record ListarManutencoesPorVeiculoQuery(Guid VeiculoId)
    : IRequest<ResultViewModel<List<ManutencaoDto>>>;
