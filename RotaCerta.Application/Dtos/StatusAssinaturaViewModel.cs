using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Dtos
{
    public record StatusAssinaturaViewModel(
        string Status,
        DateTime? TrialFimEm,
        DateTime? ProximaCobrancaEm,
        int? DiasRestantes
    );
}
