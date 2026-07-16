using Microsoft.EntityFrameworkCore;
using RotaCerta.Domain.Common.Interfaces;
using RotaCerta.Domain.Models;
using RotaCerta.Infraestructure.Context;

namespace RotaCerta.Infrastructure.Repositories;

public sealed class ManutencaoRepository(DbRotaCertaContext context) : IManutencaoRepository
{
    public async Task<Manutencao?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Manutencoes.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<List<Manutencao>> GetByVeiculoIdAsync(Guid veiculoId, CancellationToken ct = default)
        => await context.Manutencoes
            .Where(m => m.VeiculoId == veiculoId)
            .OrderByDescending(m => m.DataRealizacao)
            .ToListAsync(ct);

    public async Task AddAsync(Manutencao manutencao, CancellationToken ct = default)
        => await context.Manutencoes.AddAsync(manutencao, ct);

    public async Task UpdateAsync(Manutencao manutencao, CancellationToken ct = default)
        => context.Manutencoes.Update(manutencao);
}
