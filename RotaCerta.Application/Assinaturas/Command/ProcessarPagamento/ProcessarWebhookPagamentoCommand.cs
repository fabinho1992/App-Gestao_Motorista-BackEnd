using MediatR;
using RotaCerta.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Assinaturas.Command.ProcessarPagamento
{
    public record ProcessarWebhookPagamentoCommand(
    string Evento,
    string? GatewayAssinaturaId,
    string? PaymentId,
    string? PaymentStatus) : IRequest<ResultViewModel>;
}
