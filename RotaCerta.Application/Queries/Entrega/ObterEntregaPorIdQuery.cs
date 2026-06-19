using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.Queries.Entrega;

public record ObterEntregaPorIdQuery(Guid EntregaId) : IRequest<ResultViewModel<Domain.Models.Entrega>>;
