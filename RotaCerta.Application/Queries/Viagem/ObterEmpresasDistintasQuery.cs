using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.Queries.Viagem;

public record ObterEmpresasDistintasQuery : IRequest<ResultViewModel<List<string>>>;
