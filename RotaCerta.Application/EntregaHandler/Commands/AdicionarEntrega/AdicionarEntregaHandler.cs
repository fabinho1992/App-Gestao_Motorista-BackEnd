using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.EntregaHandler.Commands.AdicionarEntrega;

public class AdicionarEntregaHandler : IRequestHandler<AdicionarEntregaCommand, ResultViewModel<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public AdicionarEntregaHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<Guid>> Handle(
        AdicionarEntregaCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel<Guid>.Error("Usuário não autenticado.");

            var viagem = await _unitOfWork.ViagemRepository
                .GetByIdAsync(request.ViagemId, cancellationToken);

            if (viagem is null)
                return ResultViewModel<Guid>.Error("Viagem não encontrada.");

            if (viagem.MotoristaId != motoristaId)
                return ResultViewModel<Guid>.Error("Viagem não pertence ao motorista autenticado.");

            var entrega = Entrega.Criar(
                request.ViagemId,
                request.Cliente,
                request.EnderecoDestino,
                request.Observacao);

            // AdicionarEntrega muda status da viagem para EmRota
            viagem.AdicionarEntrega(entrega);

            await _unitOfWork.EntregaRepository.AddAsync(entrega, cancellationToken);
            await _unitOfWork.ViagemRepository.UpdateAsync(viagem, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel<Guid>.Success(entrega.Id);
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel<Guid>.Error(ex.Message);
        }
    }
}
