using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.MotoristaHandler.Commands.ResetarDados;

public record ResetarDadosCommand : IRequest<ResultViewModel>;
