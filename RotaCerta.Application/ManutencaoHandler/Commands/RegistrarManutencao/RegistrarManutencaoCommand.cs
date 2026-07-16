using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Enums;

namespace RotaCerta.Application.ManutencaoHandler.Commands.RegistrarManutencao;

public record RegistrarManutencaoCommand(
    Guid VeiculoId,
    TipoManutencao Tipo,
    string? Descricao,
    DateOnly DataRealizacao,
    double KmRealizacao,
    double Custo,
    string? Observacao) : IRequest<ResultViewModel<ManutencaoDto>>;
