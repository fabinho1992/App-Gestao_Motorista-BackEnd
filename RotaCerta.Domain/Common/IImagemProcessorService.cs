using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Domain.Common
{
    public interface IImagemProcessorService
    {
        Task<Stream> ProcessarAsync(Stream imagemOriginal, CancellationToken cancellationToken = default);
    }
}
