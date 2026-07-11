using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.ViagemHandler.Commands.ExcluirViagem;

public record ExcluirViagemCommand(Guid ViagemId) : IRequest<ResultViewModel>;
