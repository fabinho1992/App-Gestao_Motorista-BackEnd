using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;

namespace RotaCerta.Domain.Models;

public class Manutencao : BaseEntity
{
    private Manutencao() { }

    public Guid VeiculoId { get; private set; }
    public TipoManutencao Tipo { get; private set; }
    public string? Descricao { get; private set; }
    public DateOnly DataRealizacao { get; private set; }
    public double KmRealizacao { get; private set; }
    public double Custo { get; private set; }
    public string? Observacao { get; private set; }

    // --- factory method ---

    public static Manutencao Registrar(
        Guid veiculoId,
        TipoManutencao tipo,
        string? descricao,
        DateOnly dataRealizacao,
        double kmRealizacao,
        double custo,
        string? observacao)
    {
        if (kmRealizacao < 0)
            throw new InvalidOperationException("Km de realização não pode ser negativo.");

        if (custo < 0)
            throw new InvalidOperationException("Custo não pode ser negativo.");

        if (dataRealizacao > DateOnly.FromDateTime(DateTime.Today))
            throw new InvalidOperationException("Data de realização não pode ser futura.");

        return new Manutencao
        {
            VeiculoId = veiculoId,
            Tipo = tipo,
            Descricao = descricao,
            DataRealizacao = dataRealizacao,
            KmRealizacao = kmRealizacao,
            Custo = custo,
            Observacao = observacao
        };
    }
}
