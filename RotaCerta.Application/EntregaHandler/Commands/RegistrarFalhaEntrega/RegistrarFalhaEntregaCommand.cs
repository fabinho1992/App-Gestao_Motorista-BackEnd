using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.EntregaHandler.Commands.RegistrarFalhaEntrega;

public record RegistrarFalhaEntregaCommand(Guid EntregaId, string Motivo) : IRequest<ResultViewModel>;
