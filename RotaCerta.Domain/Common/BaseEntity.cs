using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace RotaCerta.Domain.Common;

public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CriadoEm = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
        Excluido = false;
    }

    public Guid Id { get; protected set; }
    public DateTime CriadoEm { get; protected set; }
    public DateTime AtualizadoEm { get; protected set; }
    public DateTime? DeletadoEm { get; protected set; }
    public bool Excluido { get; protected set; }

    [JsonIgnore]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => new ReadOnlyCollection<IDomainEvent>(_domainEvents);

    public void RaiseEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public void MarcarComoAtualizado() => AtualizadoEm = DateTime.UtcNow;

    public void MarcarComoExcluido()
    {
        Excluido = true;
        DeletadoEm = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
    }
}
