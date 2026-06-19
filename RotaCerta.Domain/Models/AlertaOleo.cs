using RotaCerta.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Domain.Models
{
    public record AlertaOleo(NivelAlerta Nivel, string Mensagem, double KmFaltando);

}
