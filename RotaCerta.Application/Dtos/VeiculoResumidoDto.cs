using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Dtos
{
    public class VeiculoResumoDto
    {
        public Guid Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
    }
}
