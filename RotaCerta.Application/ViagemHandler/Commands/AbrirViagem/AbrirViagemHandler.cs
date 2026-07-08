using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Services;
using RotaCerta.Domain.Viagens;

namespace RotaCerta.Application.ViagemHandler.Commands.AbrirViagem;

public class AbrirViagemHandler : IRequestHandler<AbrirViagemCommand, ResultViewModel<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public AbrirViagemHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<Guid>> Handle(
        AbrirViagemCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel<Guid>.Error("Usuário não autenticado.");

            var motorista = await _unitOfWork.MotoristaRepository
                .GetByIdAsync(motoristaId, cancellationToken);

            if (motorista is null)
                return ResultViewModel<Guid>.Error("Motorista não encontrado.");

            var veiculo = motorista.Veiculos
                .FirstOrDefault(v => v.Id == request.VeiculoId);

            if (veiculo is null)
                return ResultViewModel<Guid>.Error(
                    "Veículo não encontrado ou não pertence ao motorista.");

            var viagensAtivas = await _unitOfWork.ViagemRepository
            .GetViagensAtivasPorMotoristaAsync(motoristaId, cancellationToken);

            // verifica se alguma viagem ativa usa o veículo solicitado
            var veiculoEmUso = viagensAtivas.Any(v => v.VeiculoId == request.VeiculoId);

            if (veiculoEmUso)
                return ResultViewModel<Guid>.Error(
                    "Este veículo já possui uma viagem em andamento. " +
                    "Encerre a viagem atual antes de abrir uma nova com este veículo.");

            var viagem = Viagem.Abrir(
                motorista,
                veiculo,
                request.Origem,
                request.EmpresaContratante,
                request.KmInicial,
                request.ValorFrete,
                request.FormaPagamento);

            await _unitOfWork.ViagemRepository.AddAsync(viagem, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel<Guid>.Success(viagem.Id);
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel<Guid>.Error(ex.Message);
        }
    }
}