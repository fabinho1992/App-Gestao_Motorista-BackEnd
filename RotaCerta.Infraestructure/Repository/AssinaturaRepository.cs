using Microsoft.EntityFrameworkCore;
using RotaCerta.Domain.Common.Interfaces;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Models;
using RotaCerta.Infraestructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Infraestructure.Repository
{
    public sealed class AssinaturaRepository(DbRotaCertaContext context) : IAssinaturaRepository
    {
        public async Task AddAsync(Assinatura assinatura, CancellationToken ct = default)
        {
            await context.Assinaturas.AddAsync(assinatura, ct);
        }

        public async Task<Assinatura?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await context.Assinaturas.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Assinatura?> GetByMotoristaIdAsync(Guid motoristaId, CancellationToken ct = default)
        {
            return await context.Assinaturas.FirstOrDefaultAsync(a => a.MotoristaId == motoristaId, ct);
        }

        public async Task<List<Assinatura>> GetTrialsProximosDoFimAsync(int diasAntes, CancellationToken ct = default)
        {
            var limite = DateTime.UtcNow.AddDays(diasAntes);

            return await context.Assinaturas
                .Where(a => a.Status == StatusAssinatura.TrialAtivo
                         && !a.LembreteEnviado
                         && a.TrialFimEm <= limite
                         && a.TrialFimEm > DateTime.UtcNow)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(Assinatura assinatura, CancellationToken ct = default)
        {
            context.Assinaturas.Update(assinatura);
        }

        public async Task<Assinatura?> GetByGatewayAssinaturaIdAsync(string gatewayAssinaturaId, CancellationToken cancellationToken = default)
            => await context.Assinaturas
            .FirstOrDefaultAsync(a => a.GatewayAssinaturaId == gatewayAssinaturaId, cancellationToken);

        public async Task<List<Assinatura>> GetAssinaturasProximasDaRenovacaoAsync(int diasAntes, CancellationToken cancellationToken = default)
        {
            var limite = DateTime.UtcNow.AddDays(diasAntes);

            return await context.Assinaturas
                .Where(a => a.Status == StatusAssinatura.Ativa
                         && !a.LembreteEnviado
                         && a.ProximaCobrancaEm != null
                         && a.ProximaCobrancaEm <= limite
                         && a.ProximaCobrancaEm > DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

    }
}
