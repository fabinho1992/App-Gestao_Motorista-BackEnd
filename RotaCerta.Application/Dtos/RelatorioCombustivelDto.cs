namespace RotaCerta.Application.Dtos;

public class RelatorioCombustivelDto
{
    public int Mes { get; set; }
    public int Ano { get; set; }
    public string NomeMes { get; set; } = string.Empty;
    public double TotalGastoCombustivel { get; set; }
    public double TotalGastoPedagio { get; set; }
    public double TotalGastoAlimentacao { get; set; }
    public double TotalGastoOutros { get; set; }
    public double TotalGastosGeral { get; set; }
    public double TotalKmRodado { get; set; }
    public double MediaKmPorLitro { get; set; }
    public double TotalLitrosAbastecidos { get; set; }
    public int TotalViagensEncerradas { get; set; }
    public List<DetalheCombustivelDto> Viagens { get; set; } = new();
}

public class DetalheCombustivelDto
{
    public Guid ViagemId { get; set; }
    public string EmpresaContratante { get; set; } = string.Empty;
    public string DataEncerramento { get; set; } = string.Empty;
    public double GastoCombustivel { get; set; }
    public double GastoPedagio { get; set; }
    public double GastoAlimentacao { get; set; }
    public double GastoOutros { get; set; }
    public double TotalGastos { get; set; }
    public double KmRodado { get; set; }
    public double ValorFrete { get; set; }
    public double SaldoLiquido { get; set; }
}
