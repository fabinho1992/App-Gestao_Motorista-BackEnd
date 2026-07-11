using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Models;

namespace RotaCerta.Application.ViagemHandler.Commands.EncerrarViagem;

public record EncerrarViagemCommand(
    Guid ViagemId,
    double KmFinal,
    double GastoCombustivel,
    double GastoPedagio = 0,
    double GastoAlimentacao = 0,
    double GastoOutros = 0,
    string ObsEncerramento = "",
    double PrecoCombustivelLitro = 0) : IRequest<ResultViewModel<AlertaOleo>>;
