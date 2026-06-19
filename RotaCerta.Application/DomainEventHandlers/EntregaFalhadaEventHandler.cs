using MediatR;
using RotaCerta.Domain.Models.Eventos;

namespace RotaCerta.Application.DomainEventHandlers;

public class EntregaFalhadaEventHandler : INotificationHandler<EntregaFalhadaEvent>
{
    public Task Handle(EntregaFalhadaEvent notification, CancellationToken cancellationToken)
    {
        // ponto de extensão: disparar alerta de falha na entrega,
        // notificar empresa contratante com o motivo da falha
        return Task.CompletedTask;
    }
}
