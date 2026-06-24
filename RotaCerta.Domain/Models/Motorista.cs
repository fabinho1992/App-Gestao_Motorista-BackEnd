using RotaCerta.Domain.Common;

namespace RotaCerta.Domain.Models;

public class Motorista : BaseEntity
{
    // construtor privado — força uso do factory method
    private Motorista() { }

    public string Nome { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string Cnh { get; private set; } = string.Empty;
    public DateOnly VencimentoCnh { get; private set; }

    // um motorista pode ter vários veículos
    public List<Veiculo> Veiculos { get; private set; } = [];

    // --- factory method ---

    public static Motorista Criar(
        string nome,
        string cpf,
        string email,
        string telefone,
        string cnh,
        DateOnly vencimentoCnh)
    {
        var motorista = new Motorista
        {
            Nome = nome,
            Cpf = cpf,
            Email = email,
            Telefone = telefone,
            Cnh = cnh,
            VencimentoCnh = vencimentoCnh
        };

        return motorista;
    }

    // --- update ---

    public void Atualizar(
        string nome,
        string cpf,
        string email,
        string telefone,
        string cnh,
        DateOnly vencimentoCnh)
    {
        Nome = nome;
        Cpf = cpf;
        Email = email;
        Telefone = telefone;
        Cnh = cnh;
        VencimentoCnh = vencimentoCnh;
        MarcarComoAtualizado();
    }

    // --- métodos ---

    /// <summary>
    /// Adiciona um veículo ao motorista.
    /// </summary>
    public void AdicionarVeiculo(Veiculo veiculo)
    {
        Veiculos.Add(veiculo);
        MarcarComoAtualizado();
    }

    /// <summary>
    /// Retorna true se a CNH já venceu.
    /// </summary>
    public bool CnhVencida()
        => VencimentoCnh < DateOnly.FromDateTime(DateTime.Today);

    /// <summary>
    /// Retorna true se a CNH vence nos próximos 'dias' dias.
    /// </summary>
    public bool CnhProximaDoVencimento(int dias = 30)
        => VencimentoCnh <= DateOnly.FromDateTime(DateTime.Today.AddDays(dias));
}