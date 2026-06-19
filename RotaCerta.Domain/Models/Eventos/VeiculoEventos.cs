using RotaCerta.Domain.Common;

namespace RotaCerta.Domain.Models.Eventos;

public sealed record KmAtualizadoEvent(
    Guid VeiculoId,
    double KmAnterior,
    double KmAtual) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public sealed record TrocaOleoRegistradaEvent(
    Guid VeiculoId,
    double KmTroca,
    double ProximaTrocaKm) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}