using RotaCerta.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Domain.Services.IEmail
{
    public interface ISendEmail
    {
        Task ResetPassword(Motorista usuario, string code);
        Task LembreteTrialAsync(Motorista motorista, int diasRestantes);
        Task LembreteRenovacaoAsync(Motorista motorista, int diasRestantes);
    }
}
