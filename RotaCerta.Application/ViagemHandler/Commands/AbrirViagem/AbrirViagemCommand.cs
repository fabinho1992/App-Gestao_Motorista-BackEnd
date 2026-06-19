using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Enums;

namespace RotaCerta.Application.ViagemHandler.Commands.AbrirViagem;

public record AbrirViagemCommand(
    Guid VeiculoId,
    string Origem,
    string EmpresaContratante,
    double KmInicial,
    double ValorFrete,
    FormaPagamento FormaPagamento) : IRequest<ResultViewModel<Guid>>;
