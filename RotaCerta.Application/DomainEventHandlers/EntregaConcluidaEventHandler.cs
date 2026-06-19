using MediatR;
using RotaCerta.Domain.Models.Eventos;

namespace RotaCerta.Application.DomainEventHandlers;

public class EntregaConcluidaEventHandler : INotificationHandler<EntregaConcluidaEvent>
{
    public Task Handle(EntregaConcluidaEvent notification, CancellationToken cancellationToken)
    {
        // ponto de extensão: registrar conclusão no relatório de produtividade,
        // notificar empresa contratante, atualizar contadores de entregas
        return Task.CompletedTask;
    }
}
