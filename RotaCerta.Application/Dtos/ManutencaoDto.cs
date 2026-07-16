namespace RotaCerta.Application.Dtos;

public class ManutencaoDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateOnly DataRealizacao { get; set; }
    public double KmRealizacao { get; set; }
    public double Custo { get; set; }
    public string Observacao { get; set; } = string.Empty;
}
