using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Dtos
{
    public record CheckoutAssinaturaViewModel(string QrCodeBase64, string PixCopiaECola, string Expiracao);
}
