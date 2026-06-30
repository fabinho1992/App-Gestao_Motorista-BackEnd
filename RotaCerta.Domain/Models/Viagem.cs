using RotaCerta.Domain.Common;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Models.Eventos;

namespace RotaCerta.Domain.Viagens;

public class Viagem : BaseEntity
{
    private Viagem() { }

    public Guid MotoristaId { get; private set; }
    public Guid VeiculoId { get; private set; }
    public Veiculo? Veiculo { get; private set; }

    // --- dados preenchidos na ABERTURA ---
    public DateOnly DataSaida { get; private set; }
    public string Origem { get; private set; } = string.Empty;
    public string EmpresaContratante { get; private set; } = string.Empty;
    public double KmInicial { get; private set; }
    public double ValorFrete { get; private set; }
    public FormaPagamento FormaPagamento { get; private set; }
    public bool Pago { get; private set; }
    public StatusPagamento StatusPagamento { get; private set; } = StatusPagamento.Pendente;
    public StatusViagem Status { get; private set; } = StatusViagem.Aberta;

    // entregas feitas durante a viagem
    public List<Entrega> Entregas { get; private set; } = [];

    // --- dados preenchidos no ENCERRAMENTO ---
    public double? KmFinal { get; private set; }
    public DateOnly? DataFim { get; private set; }
    public double GastoCombustivel { get; private set; }
    public double GastoPedagio { get; private set; }
    public double GastoAlimentacao { get; private set; }
    public double GastoOutros { get; private set; }
    public string ObsEncerramento { get; private set; } = string.Empty;

    // --- factory method ---

    public static Viagem Abrir(
        Motorista motorista,
        Veiculo veiculo,
        string origem,
        string empresaContratante,
        double kmInicial,
        double valorFrete,
        FormaPagamento formaPagamento)
    {

        if (!motorista.Veiculos.Any(v => v.Id == veiculo.Id))
            throw new InvalidOperationException("Veículo não pertence ao motorista.");

        var viagem = new Viagem
        {
            MotoristaId = motorista.Id,
            VeiculoId = veiculo.Id,
            Veiculo = veiculo,
            DataSaida = DateOnly.FromDateTime(DateTime.Today),
            Origem = origem,
            EmpresaContratante = empresaContratante,
            KmInicial = kmInicial,
            ValorFrete = valorFrete,
            FormaPagamento = formaPagamento,
            Status = StatusViagem.Aberta
        };

        viagem.RaiseEvent(new ViagemAbertaEvent(
            viagem.Id,
            motorista.Id,
            veiculo.Id,
            valorFrete,
            formaPagamento));

        return viagem;
    }

    // --- métodos ---

    /// <summary>
    /// Adiciona uma entrega à viagem.
    /// Só permite se a viagem estiver aberta ou em rota.
    /// </summary>
    public void AdicionarEntrega(Entrega entrega)
    {
        if (Status == StatusViagem.Encerrada)
            throw new InvalidOperationException(
                "Não é possível adicionar entregas a uma viagem encerrada.");

        Status = StatusViagem.EmRota;
        Entregas.Add(entrega);
        MarcarComoAtualizado();
    }

    /// <summary>
    /// Encerra a viagem com os gastos do dia.
    /// Atualiza o km do veículo e levanta ViagemEncerradaEvent.
    /// </summary>
    public AlertaOleo Encerrar(
        double kmFinal,
        double gastoCombustivel,
        double gastoPedagio = 0,
        double gastoAlimentacao = 0,
        double gastoOutros = 0,
        string obsEncerramento = "")
    {
        if (Status == StatusViagem.Encerrada)
            throw new InvalidOperationException("Viagem já encerrada.");

        if (Veiculo is null)
            throw new InvalidOperationException("Veículo não carregado na viagem.");

        var entregasPendentes = Entregas
       .Any(e => e.Status == StatusEntrega.Pendente);

        if (entregasPendentes)
            throw new InvalidOperationException(
                "Não é possível encerrar a viagem com entregas pendentes. " +
                "Confirme ou registre falha em todas as entregas antes de encerrar.");

        // 1. encerra a viagem
        KmFinal = kmFinal;
        DataFim = DateOnly.FromDateTime(DateTime.Today);
        GastoCombustivel = gastoCombustivel;
        GastoPedagio = gastoPedagio;
        GastoAlimentacao = gastoAlimentacao;
        GastoOutros = gastoOutros;
        ObsEncerramento = obsEncerramento;
        Status = StatusViagem.Encerrada;
        MarcarComoAtualizado();

        // 2. atualiza km do veículo — encadeia KmAtualizadoEvent no Veiculo
        Veiculo.AtualizarKm(kmFinal);

        // 3. verifica alerta de óleo automaticamente
        var alerta = Veiculo.VerificarAlertaOleo();

        // 4. levanta o evento da viagem encerrada
        RaiseEvent(new ViagemEncerradaEvent(
            Id,
            VeiculoId,
            kmFinal,
            SaldoLiquido,
            alerta));

        return alerta;
    }

    /// <summary>
    /// Registra o pagamento do frete.
    /// </summary>
    public void RegistrarPagamento()
    {
        Pago = true;
        MarcarComoAtualizado();
    }

    public void AtualizarStatusPagamento(StatusPagamento novoStatus)
    {
        StatusPagamento = novoStatus;
        MarcarComoAtualizado();
    }

    // --- propriedades calculadas ---

    /// <summary>Total de gastos da viagem.</summary>
    public double TotalGastos
        => GastoCombustivel + GastoPedagio + GastoAlimentacao + GastoOutros;

    /// <summary>Lucro líquido: frete menos todos os gastos.</summary>
    public double SaldoLiquido
        => ValorFrete - TotalGastos;

    /// <summary>Km total rodado na viagem.</summary>
    public double? KmRodado
        => KmFinal.HasValue ? KmFinal - KmInicial : null;

    /// <summary>Quantidade de entregas concluídas.</summary>
    public int TotalEntregasRealizadas
        => Entregas.Count(e => e.Status == StatusEntrega.Entregue);
}