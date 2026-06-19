using MediatR;
using RotaCerta.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace RotaCerta.Application.EntregaHandler.Commands.ConfirmarEntrega;

public record ConfirmarEntregaCommand(Guid EntregaId, List<IFormFile>? Fotos) : IRequest<ResultViewModel>;
