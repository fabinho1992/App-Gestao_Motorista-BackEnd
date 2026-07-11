using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.EntregaHandler.Commands.ExcluirEntrega;

public record ExcluirEntregaCommand(Guid EntregaId) : IRequest<ResultViewModel>;
