using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Domain.Models
{
    public class Assinatura : BaseEntity
    {
        public Guid MotoristaId { get; private set; }
        public StatusAssinatura Status { get; private set; }
        public DateTime TrialFimEm { get; private set; }
        public DateTime? ProximaCobrancaEm { get; private set; }
        public string? GatewayClienteId { get; private set; }
        public string? GatewayAssinaturaId { get; private set; }
        public bool LembreteEnviado { get; private set; }

        protected Assinatura() { }

        public Assinatura(Guid motoristaId, int diasTrial = 15)
        {
            MotoristaId = motoristaId;
            Status = StatusAssinatura.TrialAtivo;
            TrialFimEm = DateTime.UtcNow.AddDays(diasTrial);
        }

        public void VincularGateway(string clienteId, string assinaturaId)
            => (GatewayClienteId, GatewayAssinaturaId) = (clienteId, assinaturaId);

        public void ConfirmarPagamento()
        {
            Status = StatusAssinatura.Ativa;
            ProximaCobrancaEm = DateTime.UtcNow.Date.AddMonths(1);
            LembreteEnviado = false;
        }
        public void MarcarInadimplente() => Status = StatusAssinatura.Inadimplente;
        public void Cancelar() => Status = StatusAssinatura.Cancelada;
        public void MarcarLembreteEnviado() => LembreteEnviado = true;

        public bool TemAcessoLiberado() =>
            Status == StatusAssinatura.Ativa ||
            (Status == StatusAssinatura.TrialAtivo && TrialFimEm > DateTime.UtcNow);
    }
}
