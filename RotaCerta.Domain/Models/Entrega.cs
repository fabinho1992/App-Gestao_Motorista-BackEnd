using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Models.Eventos;

namespace RotaCerta.Domain.Models;

public class Entrega : BaseEntity
{
    private Entrega() { }

    public Guid ViagemId { get; private set; }
    public string Cliente { get; private set; } = string.Empty;
    public string EnderecoDestino { get; private set; } = string.Empty;
    public StatusEntrega Status { get; private set; } = StatusEntrega.Pendente;
    public string Observacao { get; private set; } = string.Empty;
    public DateTime? DataHoraEntrega { get; private set; }

    // paths/URLs das fotos do comprovante
    public List<string> Fotos { get; private set; } = [];

    // --- factory method ---

    public static Entrega Criar(
        Guid viagemId,
        string cliente,
        string enderecoDestino,
        string observacao = "")
    {
        var entrega = new Entrega
        {
            ViagemId = viagemId,
            Cliente = cliente,
            EnderecoDestino = enderecoDestino,
            Observacao = observacao
        };

        return entrega;
    }

    // --- métodos ---

    /// <summary>
    /// Confirma a entrega e registra o horário.
    /// Levanta EntregaConcluidaEvent.
    /// </summary>
    public void ConfirmarEntrega(string? fotoUrl = null)
    {
        Status = StatusEntrega.Entregue;
        DataHoraEntrega = DateTime.UtcNow;
        MarcarComoAtualizado();

        // adiciona a foto se foi enviada
        if (!string.IsNullOrEmpty(fotoUrl))
            Fotos.Add(fotoUrl);

        RaiseEvent(new EntregaConcluidaEvent(Id, ViagemId, Cliente));
    }

    /// <summary>
    /// Registra tentativa de entrega sem sucesso.
    /// Levanta EntregaFalhadaEvent.
    /// </summary>
    public void RegistrarFalha(string motivo)
    {
        Status = StatusEntrega.TentativaFalha;
        Observacao = motivo;
        DataHoraEntrega = DateTime.UtcNow;
        MarcarComoAtualizado();

        RaiseEvent(new EntregaFalhadaEvent(Id, ViagemId, Cliente, motivo));
    }

    /// <summary>
    /// Adiciona a URL/path de uma foto do comprovante.
    /// </summary>
    public void AdicionarFoto(string fotoUrl)
    {
        if (string.IsNullOrWhiteSpace(fotoUrl))
            throw new ArgumentException("URL da foto não pode ser vazia.");

        Fotos.Add(fotoUrl);
        MarcarComoAtualizado();
    }
}