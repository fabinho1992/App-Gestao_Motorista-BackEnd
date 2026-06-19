using MediatR;
using RotaCerta.Application.Common;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Enums;

namespace RotaCerta.Application.Queries.Viagem;

public class ListarViagensPorMotoristaQuery : ParametrosPaginacao,
    IRequest<ResultViewModel<List<Domain.Viagens.Viagem>>>
{
    public StatusViagem? Status { get; set; }
}
