using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Dtos
{
    // ViagemDto — sem ciclo
    public class ViagemDto
    {
        public Guid Id { get; set; }
        public string EmpresaContratante { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public DateOnly DataSaida { get; set; }
        public double KmInicial { get; set; }
        public double? KmFinal { get; set; }
        public double ValorFrete { get; set; }
        public string FormaPagamento { get; set; } = string.Empty;
        public bool Pago { get; set; }
        public string Status { get; set; } = string.Empty;
        public double GastoCombustivel { get; set; }
        public double GastoPedagio { get; set; }
        public double GastoAlimentacao { get; set; }
        public double GastoOutros { get; set; }
        public string ObsEncerramento { get; set; } = string.Empty;
        public double TotalGastos { get; set; }
        public double SaldoLiquido { get; set; }
        public double? KmRodado { get; set; }
        public int TotalEntregasRealizadas { get; set; }

        public VeiculoResumoDto? Veiculo { get; set; }
        public List<EntregaDto> Entregas { get; set; } = [];
    }
}
