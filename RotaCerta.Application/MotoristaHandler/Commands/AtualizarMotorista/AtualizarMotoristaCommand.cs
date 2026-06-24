using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.MotoristaHandler.Commands.AtualizarMotorista;

public record AtualizarMotoristaCommand(
    string Nome,
    string Cpf,
    string Email,
    string Telefone,
    string Cnh,
    DateOnly VencimentoCnh) : IRequest<ResultViewModel>;
