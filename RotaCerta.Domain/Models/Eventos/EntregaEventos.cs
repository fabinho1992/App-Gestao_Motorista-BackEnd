using RotaCerta.Domain.Common;

namespace RotaCerta.Domain.Models.Eventos;

public sealed record EntregaConcluidaEvent(
    Guid EntregaId,
    Guid ViagemId,
    string Cliente) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public sealed record EntregaFalhadaEvent(
    Guid EntregaId,
    Guid ViagemId,
    string Cliente,
    string Motivo) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}