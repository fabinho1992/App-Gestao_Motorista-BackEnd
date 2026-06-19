using MediatR;
using RotaCerta.Application.Common;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.Queries.Entrega;

public class ListarEntregasPorViagemQuery : ParametrosPaginacao,
    IRequest<ResultViewModel<List<Domain.Models.Entrega>>>
{
    public Guid ViagemId { get; set; }

    public ListarEntregasPorViagemQuery() { }

    public ListarEntregasPorViagemQuery(Guid viagemId)
    {
        ViagemId = viagemId;
    }
}
