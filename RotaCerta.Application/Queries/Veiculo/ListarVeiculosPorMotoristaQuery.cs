using MediatR;
using RotaCerta.Application.Common;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.Queries.Veiculo;

public class ListarVeiculosPorMotoristaQuery : ParametrosPaginacao,
    IRequest<ResultViewModel<List<Domain.Models.Veiculo>>>
{
}
