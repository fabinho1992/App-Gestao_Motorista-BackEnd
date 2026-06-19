using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.EntregaHandler.Commands.AdicionarEntrega;

public record AdicionarEntregaCommand(
    Guid ViagemId,
    string Cliente,
    string EnderecoDestino,
    string Observacao = "") : IRequest<ResultViewModel<Guid>>;
