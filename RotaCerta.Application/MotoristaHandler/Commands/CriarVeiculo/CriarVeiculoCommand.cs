using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Enums;

namespace RotaCerta.Application.MotoristaHandler.Commands.CriarVeiculo;

public record CriarVeiculoCommand(
    string Placa,
    string Modelo,
    int Ano,
    TipoCombustivel TipoCombustivel,
    double KmAtual,
    double KmUltimoOleo,
    DateOnly DataUltimoOleo,
    double IntervaloOleo = 5000) : IRequest<ResultViewModel<Guid>>;
