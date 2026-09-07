using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Assinaturas.Command.ProcessarPagamento
{
    public class ProcessarWebhookPagamentoHandler : IRequestHandler<ProcessarWebhookPagamentoCommand, ResultViewModel>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProcessarWebhookPagamentoHandler(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        } 

        public async Task<ResultViewModel> Handle(ProcessarWebhookPagamentoCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.GatewayAssinaturaId))
                return ResultViewModel.Success();

            var assinatura = await _unitOfWork.AssinaturaRepository
                .GetByGatewayAssinaturaIdAsync(request.GatewayAssinaturaId, cancellationToken);

            if (assinatura is null)
                return ResultViewModel.Success();

            switch (request.Evento)
            {
                case "PAYMENT_CONFIRMED":
                case "PAYMENT_RECEIVED":
                    assinatura.ConfirmarPagamento();
                    break;
                case "PAYMENT_OVERDUE":
                    assinatura.MarcarInadimplente();
                    break;
                default:
                    return ResultViewModel.Success();
            }

            await _unitOfWork.AssinaturaRepository.UpdateAsync(assinatura, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel.Success();
        }
    }
}
