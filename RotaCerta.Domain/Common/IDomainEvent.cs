using MediatR;

namespace RotaCerta.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OcorridoEm { get; }
}
