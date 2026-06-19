using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Dtos
{
    public class EntregaDto
    {
        public Guid Id { get; set; }
        public Guid ViagemId { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string EnderecoDestino { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public DateTime? DataHoraEntrega { get; set; }
        public List<string> Fotos { get; set; } = [];
    }
}
