using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Models;

namespace RotaCerta.Domain.Models.Eventos;

public sealed record ViagemAbertaEvent(
    Guid ViagemId,
    Guid MotoristaId,
    Guid VeiculoId,
    double ValorFrete,
    FormaPagamento FormaPagamento) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public sealed record ViagemEncerradaEvent(
    Guid ViagemId,
    Guid VeiculoId,
    double KmFinal,
    double SaldoLiquido,
    AlertaOleo AlertaOleo) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}