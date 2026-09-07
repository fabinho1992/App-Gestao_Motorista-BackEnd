using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Domain.Services.Pagamento.Dtos
{
    public record PixQrCodeDto(string EncodedImage, string Payload, string ExpirationDate);
}
