using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Viagens;

namespace RotaCerta.Application.Queries.Viagem;

public record ObterViagemPorIdQuery(Guid ViagemId) : IRequest<ResultViewModel<ViagemDto>>;
