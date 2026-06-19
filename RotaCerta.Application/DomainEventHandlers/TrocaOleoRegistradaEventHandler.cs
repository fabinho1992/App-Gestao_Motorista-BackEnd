using MediatR;
using RotaCerta.Domain.Models.Eventos;

namespace RotaCerta.Application.DomainEventHandlers;

public class TrocaOleoRegistradaEventHandler : INotificationHandler<TrocaOleoRegistradaEvent>
{
    public Task Handle(TrocaOleoRegistradaEvent notification, CancellationToken cancellationToken)
    {
        // ponto de extensão: agendar lembrete para a próxima troca
        // notification.ProximaTrocaKm indica o km alvo da próxima troca
        return Task.CompletedTask;
    }
}
