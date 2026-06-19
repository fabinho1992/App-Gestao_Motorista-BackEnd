using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Models.Eventos;
using RotaCerta.Domain.Viagens;

namespace RotaCerta.Domain.Models;

public class Veiculo : BaseEntity
{
    private Veiculo() { }

    public Guid MotoristaId { get; private set; }
    public string Placa { get; private set; } = string.Empty;
    public string Modelo { get; private set; } = string.Empty;
    public int Ano { get; private set; }
    public TipoCombustivel TipoCombustivel { get; private set; }
    public double KmAtual { get; private set; }

    // --- dados de troca de óleo ---
    public double KmUltimoOleo { get; private set; }
    public DateOnly DataUltimoOleo { get; private set; }
    public double IntervaloOleo { get; private set; } = 5000;

    public List<Viagem> Viagens { get; private set; } = [];

    // --- factory method ---

    public static Veiculo Criar(
        Guid motoristaId,
        string placa,
        string modelo,
        int ano,
        TipoCombustivel tipoCombustivel,
        double kmAtual,
        double kmUltimoOleo,
        DateOnly dataUltimoOleo,
        double intervaloOleo = 5000)
    {
        var veiculo = new Veiculo
        {
            MotoristaId = motoristaId,
            Placa = placa,
            Modelo = modelo,
            Ano = ano,
            TipoCombustivel = tipoCombustivel,
            KmAtual = kmAtual,
            KmUltimoOleo = kmUltimoOleo,
            DataUltimoOleo = dataUltimoOleo,
            IntervaloOleo = intervaloOleo
        };

        return veiculo;
    }

    // --- métodos ---

    /// <summary>
    /// Atualiza o km atual. Chamado ao encerrar viagem.
    /// Levanta KmAtualizadoEvent.
    /// </summary>
    public void AtualizarKm(double kmFinal)
    {
        if (kmFinal < KmAtual)
            throw new InvalidOperationException(
                $"Km final ({kmFinal}) não pode ser menor que o km atual ({KmAtual}).");

        var kmAnterior = KmAtual;
        KmAtual = kmFinal;
        MarcarComoAtualizado();

        RaiseEvent(new KmAtualizadoEvent(Id, kmAnterior, KmAtual));
    }

    /// <summary>
    /// Verifica o nível do alerta de troca de óleo.
    /// Chamado automaticamente ao encerrar viagem.
    /// </summary>
    public AlertaOleo VerificarAlertaOleo()
    {
        var kmRodado = KmAtual - KmUltimoOleo;
        var kmFaltando = IntervaloOleo - kmRodado;

        return kmFaltando switch
        {
            <= 0 => new AlertaOleo(
                           NivelAlerta.Vermelho,
                           $"Troca de óleo vencida! Passou {Math.Abs(kmFaltando):F0} km.",
                           kmFaltando),

            <= 1000 => new AlertaOleo(
                           NivelAlerta.Amarelo,
                           $"Trocar óleo em breve. Faltam {kmFaltando:F0} km.",
                           kmFaltando),

            _ => new AlertaOleo(
                           NivelAlerta.Verde,
                           $"Óleo ok. Próxima troca em {kmFaltando:F0} km.",
                           kmFaltando)
        };
    }

    /// <summary>
    /// Registra a troca de óleo e reseta o contador.
    /// Levanta TrocaOleoRegistradaEvent.
    /// </summary>
    public AlertaOleo RegistrarTrocaOleo()
    {
        KmUltimoOleo = KmAtual;
        DataUltimoOleo = DateOnly.FromDateTime(DateTime.Today);
        MarcarComoAtualizado();

        RaiseEvent(new TrocaOleoRegistradaEvent(Id, KmAtual, KmAtual + IntervaloOleo));

        return VerificarAlertaOleo();
    }
}