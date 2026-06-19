using MediatR;
using RotaCerta.Application.Dtos;

namespace RotaCerta.Application.MotoristaHandler.Commands.CriarMotorista;

public record CriarMotoristaCommand(
    string Nome,
    string Cpf,
    string Cnh,
    string telefone,
    DateOnly vencimentoCnh,
    string Email,
    string senha) : IRequest<ResultViewModel<Guid>>;
