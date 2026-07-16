using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.ManutencaoHandler.Commands.ExcluirManutencao;

public record ExcluirManutencaoCommand(Guid ManutencaoId) : IRequest<ResultViewModel>;
