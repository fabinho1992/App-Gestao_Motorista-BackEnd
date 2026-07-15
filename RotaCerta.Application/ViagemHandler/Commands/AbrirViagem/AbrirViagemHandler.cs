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

            if (request.KmInicial < 0)
                return ResultViewModel<Guid>.Error(
                    "Km inicial não pode ser negativo.");

            if (request.ValorFrete <= 0)
                return ResultViewModel<Guid>.Error(
                    "Valor do frete deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(request.EmpresaContratante))
                return ResultViewModel<Guid>.Error(
                    "Empresa contratante é obrigatória.");

            if (string.IsNullOrWhiteSpace(request.Origem))
                return ResultViewModel<Guid>.Error(
                    "Origem é obrigatória.");

            if (request.KmInicial < veiculo.KmAtual)
                return ResultViewModel<Guid>.Error(
                    $"Km inicial não pode ser menor que o km atual do veículo ({veiculo.KmAtual:N0} km).");

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