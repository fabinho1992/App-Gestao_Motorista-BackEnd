using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.Queries.Dashboard;

public record ObterRelatorioCombustivelQuery(int Mes, int Ano)
    : IRequest<ResultViewModel<RelatorioCombustivelDto>>;
