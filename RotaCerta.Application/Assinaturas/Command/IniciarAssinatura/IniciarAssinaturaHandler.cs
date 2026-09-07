using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;
using RotaCerta.Domain.Services.Pagamento;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Assinaturas.Command.IniciarAssinatura
{
    public class IniciarAssinaturaHandler : IRequestHandler<IniciarAssinaturaCommand, ResultViewModel<CheckoutAssinaturaViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioContext _usuarioContext;
        private readonly IPagamentoGatewayService _pagamentoGatewayService;
        private const decimal ValorMensalidade = 9.99m;

        public IniciarAssinaturaHandler(
            IUnitOfWork unitOfWork, IUsuarioContext usuarioContext, IPagamentoGatewayService pagamentoGatewayService)
        {
            _unitOfWork = unitOfWork;
            _usuarioContext = usuarioContext;
            _pagamentoGatewayService = pagamentoGatewayService;
        }

        public async Task<ResultViewModel<CheckoutAssinaturaViewModel>> Handle(
            IniciarAssinaturaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                    return ResultViewModel<CheckoutAssinaturaViewModel>.Error("Usuário não autenticado.");

                var motorista = await _unitOfWork.MotoristaRepository.GetByIdAsync(motoristaId, cancellationToken);
                if (motorista is null)
                    return ResultViewModel<CheckoutAssinaturaViewModel>.Error("Motorista não encontrado.");

                var assinatura = await _unitOfWork.AssinaturaRepository.GetByMotoristaIdAsync(motoristaId, cancellationToken);
                if (assinatura is null)
                    return ResultViewModel<CheckoutAssinaturaViewModel>.Error("Assinatura não encontrada.");

                if (string.IsNullOrEmpty(assinatura.GatewayAssinaturaId))
                {
                    var clienteId = await _pagamentoGatewayService.CriarClienteAsync(
                        motorista.Nome, motorista.Cpf, motorista.Email, motorista.Telefone, cancellationToken);

                    var assinaturaGatewayId = await _pagamentoGatewayService.CriarAssinaturaAsync(
                        clienteId, ValorMensalidade, "RotaCerta - Mensalidade", cancellationToken);

                    assinatura.VincularGateway(clienteId, assinaturaGatewayId);
                    await _unitOfWork.AssinaturaRepository.UpdateAsync(assinatura, cancellationToken);
                    await _unitOfWork.CommitAsync(cancellationToken);
                }

                var cobrancaId = await _pagamentoGatewayService.ObterPrimeiraCobrancaIdAsync(
                    assinatura.GatewayAssinaturaId!, cancellationToken);

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
