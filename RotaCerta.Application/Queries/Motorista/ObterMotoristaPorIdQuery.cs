using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Models;

namespace RotaCerta.Application.Queries.Motorista;

public record ObterMotoristaPorIdQuery() : IRequest<ResultViewModel<Domain.Models.Motorista>>;
