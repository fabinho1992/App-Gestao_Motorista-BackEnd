using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Domain.Services
{
    public interface IWhatsAppService
    {
        Task EnviarLembreteTrialAsync(string numeroTelefone, string nomeMotorista, int diasRestantes, CancellationToken cancellationToken = default);

        Task EnviarLembreteRenovacaoAsync(string numeroTelefone, string nomeMotorista, int diasRestantes, CancellationToken cancellationToken = default);
    }
}
