namespace RotaCerta.Application.Dtos;

public class VeiculoComAlertaDto
{
    public Guid Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string TipoCombustivel { get; set; } = string.Empty;
    public double KmAtual { get; set; }
    public double KmUltimoOleo { get; set; }
    public DateOnly DataUltimoOleo { get; set; }
    public double IntervaloOleo { get; set; }
    public AlertaOleoDto AlertaOleo { get; set; } = new();

    public List<ViagemResumoDto> Viagens { get; set; } = [];
}

public class AlertaOleoDto
{
    public string Nivel { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public double KmFaltando { get; set; }
}

public class ViagemResumoDto
{
    public Guid Id { get; set; }
    public string EmpresaContratante { get; set; } = string.Empty;
    public DateOnly DataSaida { get; set; }
    public string Status { get; set; } = string.Empty;
    public double ValorFrete { get; set; }
    public double SaldoLiquido { get; set; }
    public double? KmRodado { get; set; }
    public int TotalEntregas { get; set; }
}
