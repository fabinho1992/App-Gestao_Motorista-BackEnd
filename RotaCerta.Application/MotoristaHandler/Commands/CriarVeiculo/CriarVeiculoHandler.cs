using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.MotoristaHandler.Commands.CriarVeiculo;

public class CriarVeiculoHandler : IRequestHandler<CriarVeiculoCommand, ResultViewModel<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public CriarVeiculoHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<Guid>> Handle(
        CriarVeiculoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel<Guid>.Error("Usuário não autenticado.");

            Console.WriteLine($"MotoristaId: {motoristaId}");

            var motorista = await _unitOfWork.MotoristaRepository
                .GetByIdAsync(motoristaId, cancellationToken);

            if (motorista is null)
                return ResultViewModel<Guid>.Error("Motorista não encontrado.");

            if (string.IsNullOrWhiteSpace(request.Placa))
                return ResultViewModel<Guid>.Error(
                    "Placa é obrigatória.");

            if (string.IsNullOrWhiteSpace(request.Modelo))
                return ResultViewModel<Guid>.Error(
                    "Modelo é obrigatório.");

            var anoAtual = DateTime.Today.Year;
            if (request.Ano < 1950 || request.Ano > anoAtual + 1)
                return ResultViewModel<Guid>.Error(
                    $"Ano do veículo inválido. Informe um ano entre 1950 e {anoAtual + 1}.");

            if (request.KmAtual < 0)
                return ResultViewModel<Guid>.Error(
                    "Km atual não pode ser negativo.");

            if (request.KmUltimoOleo < 0)
                return ResultViewModel<Guid>.Error(
                    "Km do último óleo não pode ser negativo.");

            if (request.KmUltimoOleo > request.KmAtual)
                return ResultViewModel<Guid>.Error(
                    "Km do último óleo não pode ser maior que o km atual.");

            if (request.DataUltimoOleo > DateOnly.FromDateTime(DateTime.Today))
                return ResultViewModel<Guid>.Error(
                    "Data do último óleo não pode ser uma data futura.");

            if (request.IntervaloOleo <= 0)
                return ResultViewModel<Guid>.Error(
                    "Intervalo de troca de óleo deve ser maior que zero.");

            var veiculo = Veiculo.Criar(
                motoristaId,
                request.Placa,
                request.Modelo,
                request.Ano,
                request.TipoCombustivel,
                request.KmAtual,
                request.KmUltimoOleo,
                request.DataUltimoOleo,
                request.IntervaloOleo);

            await _unitOfWork.VeiculoRepository.AddAsync(veiculo, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel<Guid>.Success(veiculo.Id);
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel<Guid>.Error(ex.Message);
        }
    }
}
