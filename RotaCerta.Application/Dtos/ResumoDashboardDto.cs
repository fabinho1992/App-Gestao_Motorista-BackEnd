namespace RotaCerta.Application.Dtos;

public class ResumoDashboardDto
{
    public double TotalGanhosMes { get; set; }
    public double TotalGastosMes { get; set; }
    public double LucroLiquidoMes { get; set; }
    public double TotalKmRodadosMes { get; set; }
    public int TotalViagensMes { get; set; }
    public int TotalEntregasMes { get; set; }
    public bool TemViagemAtiva { get; set; }
    public Guid? ViagemAtivaId { get; set; }
    public string? ViagemAtivaEmpresa { get; set; }
    public string? ViagemAtivaStatus { get; set; }
    public int EntregasConcluidasAtiva { get; set; }
    public int TotalEntregasAtiva { get; set; }
    public double TotalAReceber { get; set; }
    public int ViagensPendentePagamento { get; set; }
    public double TotalGastosPendentes { get; set; }
    public double LucroEstimadoPendente { get; set; }
}
