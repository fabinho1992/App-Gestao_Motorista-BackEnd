using RotaCerta.Domain.Services.Pagamento.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Domain.Services.Pagamento
{
    public interface IPagamentoGatewayService
    {
        Task<string> CriarClienteAsync(string nome, string cpf, string email, string telefone, CancellationToken cancellationToken = default);
        Task<string> CriarAssinaturaAsync(string clienteId, decimal valor, string descricao, CancellationToken cancellationToken = default);
        Task<string> ObterPrimeiraCobrancaIdAsync(string assinaturaId, CancellationToken cancellationToken = default);
        Task<PixQrCodeDto> ObterQrCodePixAsync(string cobrancaId, CancellationToken cancellationToken = default);
        Task<string> ObterCobrancaEmAbertoAsync(string assinaturaId, CancellationToken cancellationToken = default);
    }
}
