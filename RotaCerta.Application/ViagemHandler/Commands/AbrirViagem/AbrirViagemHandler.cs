using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
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

            // carrega motorista com seus veículos
            var motorista = await _unitOfWork.MotoristaRepository
                .GetByIdAsync(motoristaId, cancellationToken);

            if (motorista is null)
                return ResultViewModel<Guid>.Error("Motorista não encontrado.");

            var veiculo = motorista.Veiculos.FirstOrDefault(v => v.Id == request.VeiculoId);

            if (veiculo is null)
                return ResultViewModel<Guid>.Error("Veículo não encontrado ou não pertence ao motorista.");

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
