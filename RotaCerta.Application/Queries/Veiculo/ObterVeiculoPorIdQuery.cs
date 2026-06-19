using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.Queries.Veiculo;

public record ObterVeiculoPorIdQuery(Guid VeiculoId)
    : IRequest<ResultViewModel<VeiculoComAlertaDto>>;
