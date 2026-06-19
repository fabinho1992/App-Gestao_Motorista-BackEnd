using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.Queries.Dashboard;

public record ObterResumoDashboardQuery(int Mes, int Ano)
    : IRequest<ResultViewModel<ResumoDashboardDto>>;
