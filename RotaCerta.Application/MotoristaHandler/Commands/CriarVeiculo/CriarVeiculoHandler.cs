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
