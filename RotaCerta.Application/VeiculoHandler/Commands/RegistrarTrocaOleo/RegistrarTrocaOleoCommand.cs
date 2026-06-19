using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Models;

namespace RotaCerta.Application.VeiculoHandler.Commands.RegistrarTrocaOleo;

public record RegistrarTrocaOleoCommand(Guid VeiculoId) : IRequest<ResultViewModel<AlertaOleo>>;
