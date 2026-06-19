using Microsoft.EntityFrameworkCore;
using RotaCerta.Domain.Common.Interfaces;
using RotaCerta.Domain.Models;
using RotaCerta.Infraestructure.Context;

namespace RotaCerta.Infrastructure.Repositories;

public sealed class EntregaRepository(DbRotaCertaContext context) : IEntregaRepository
{
    public async Task<Entrega?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Entregas
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<(List<Entrega> Items, int TotalPaginas, int TotalCount)> GetByViagemIdAsync(
        Guid viagemId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = context.Entregas
            .Where(e => e.ViagemId == viagemId);

        var totalCount = await query.CountAsync(ct);
        var totalPaginas = (int)Math.Ceiling((double)totalCount / pageSize);
        var items = await query
            .OrderBy(e => e.CriadoEm)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalPaginas, totalCount);
    }

    public async Task AddAsync(Entrega entrega, CancellationToken ct = default)
        => await context.Entregas.AddAsync(entrega, ct);

    public async Task UpdateAsync(Entrega entrega, CancellationToken ct = default)
        => context.Entregas.Update(entrega);
}