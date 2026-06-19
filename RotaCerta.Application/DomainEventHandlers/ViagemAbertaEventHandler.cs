using MediatR;
using RotaCerta.Domain.Models.Eventos;

namespace RotaCerta.Application.DomainEventHandlers;

public class ViagemAbertaEventHandler : INotificationHandler<ViagemAbertaEvent>
{
    public Task Handle(ViagemAbertaEvent notification, CancellationToken cancellationToken)
    {
        // ponto de extensão: notificação push, log de auditoria, analytics
        return Task.CompletedTask;
    }
}
