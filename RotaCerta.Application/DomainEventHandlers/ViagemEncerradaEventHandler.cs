using MediatR;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Models.Eventos;

namespace RotaCerta.Application.DomainEventHandlers;

public class ViagemEncerradaEventHandler : INotificationHandler<ViagemEncerradaEvent>
{
    public Task Handle(ViagemEncerradaEvent notification, CancellationToken cancellationToken)
    {
        // ponto de extensão: se AlertaOleo.Nivel for Amarelo ou Vermelho,
        // enviar notificação push para o motorista avisar sobre troca de óleo
        if (notification.AlertaOleo.Nivel != NivelAlerta.Verde)
        {
            // futura integração: INotificacaoService.EnviarAlertaOleoAsync(...)
        }

        return Task.CompletedTask;
    }
}
