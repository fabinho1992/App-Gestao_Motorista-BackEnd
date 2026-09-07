using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;
using RotaCerta.Domain.Services.Pagamento;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Assinaturas.Command.RenovarAssinatura
{
    public class RenovarAssinaturaHandler : IRequestHandler<RenovarAssinaturaCommand, ResultViewModel<CheckoutAssinaturaViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioContext _usuarioContext;
        private readonly IPagamentoGatewayService _pagamentoGatewayService;

        public RenovarAssinaturaHandler(
            IUnitOfWork unitOfWork, IUsuarioContext usuarioContext, IPagamentoGatewayService pagamentoGatewayService)
        {
            _unitOfWork = unitOfWork;
            _usuarioContext = usuarioContext;
            _pagamentoGatewayService = pagamentoGatewayService;
        }

        public async Task<ResultViewModel<CheckoutAssinaturaViewModel>> Handle(
            RenovarAssinaturaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                    return ResultViewModel<CheckoutAssinaturaViewModel>.Error("Usuário não autenticado.");

                var assinatura = await _unitOfWork.AssinaturaRepository.GetByMotoristaIdAsync(motoristaId, cancellationToken);
                if (assinatura is null)
                    return ResultViewModel<CheckoutAssinaturaViewModel>.Error("Assinatura não encontrada.");

                if (string.IsNullOrEmpty(assinatura.GatewayAssinaturaId))
                    return ResultViewModel<CheckoutAssinaturaViewModel>.Error("Assinatura ainda não foi iniciada no gateway de pagamento.");

                var cobrancaId = await _pagamentoGatewayService.ObterCobrancaEmAbertoAsync(
                    assinatura.GatewayAssinaturaId, cancellationToken);

                var qrCode = await _pagamentoGatewayService.ObterQrCodePixAsync(cobrancaId, cancellationToken);

                return ResultViewModel<CheckoutAssinaturaViewModel>.Success(
                    new CheckoutAssinaturaViewModel(qrCode.EncodedImage, qrCode.Payload, qrCode.ExpirationDate));
            }
            catch (InvalidOperationException ex)
            {
                return ResultViewModel<CheckoutAssinaturaViewModel>.Error(ex.Message);
            }
        }
    }
}
