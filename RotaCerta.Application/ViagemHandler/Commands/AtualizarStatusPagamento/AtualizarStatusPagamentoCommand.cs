using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Enums;

namespace RotaCerta.Application.ViagemHandler.Commands.AtualizarStatusPagamento;

public record AtualizarStatusPagamentoCommand(
    Guid ViagemId,
    StatusPagamento NovoStatus)
    : IRequest<ResultViewModel>;
