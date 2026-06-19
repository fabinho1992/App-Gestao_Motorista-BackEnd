using MediatR;
using RotaCerta.Domain.Common;

namespace RotaCerta.Infraestructure.DomainEvents;

public sealed class DomainEventDispatcher(IPublisher publisher)
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in events)
            await publisher.Publish(domainEvent, cancellationToken);
    }
}
