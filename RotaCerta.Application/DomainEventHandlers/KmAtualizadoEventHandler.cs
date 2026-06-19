using MediatR;
using RotaCerta.Domain.Models.Eventos;

namespace RotaCerta.Application.DomainEventHandlers;

public class KmAtualizadoEventHandler : INotificationHandler<KmAtualizadoEvent>
{
    public Task Handle(KmAtualizadoEvent notification, CancellationToken cancellationToken)
    {
        // ponto de extensão: atualizar dashboard de estatísticas, histórico de km
        return Task.CompletedTask;
    }
}
