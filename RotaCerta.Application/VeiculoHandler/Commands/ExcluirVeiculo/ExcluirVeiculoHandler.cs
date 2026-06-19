using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.VeiculoHandler.Commands.ExcluirVeiculo
{
    public class ExcluirVeiculoHandler
    : IRequestHandler<ExcluirVeiculoCommand, ResultViewModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioContext _usuarioContext;

        public async Task<ResultViewModel> Handle(
            ExcluirVeiculoCommand request,
            CancellationToken ct)
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel.Error("Não autenticado.");

            var veiculo = await _unitOfWork.VeiculoRepository
                .GetByIdAsync(request.VeiculoId, ct);

            if (veiculo is null)
                return ResultViewModel.Error("Veículo não encontrado.");

            if (veiculo.MotoristaId != motoristaId)
                return ResultViewModel.Error("Veículo não pertence ao motorista.");

            // não deleta do banco — só marca como excluído
            veiculo.MarcarComoExcluido();

            await _unitOfWork.VeiculoRepository.UpdateAsync(veiculo, ct);
            await _unitOfWork.CommitAsync(ct);

            return ResultViewModel.Success();
        }
    }
}
