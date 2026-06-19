using Microsoft.EntityFrameworkCore;
using RotaCerta.Domain.Common.Interfaces;
using RotaCerta.Domain.Models;
using RotaCerta.Infraestructure.Context;

namespace RotaCerta.Infrastructure.Repositories;

public sealed class VeiculoRepository(DbRotaCertaContext context) : IVeiculoRepository
{
    public async Task<Veiculo?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Veiculos.Include(v => v.Viagens).ThenInclude(vi => vi.Entregas)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<(List<Veiculo> Items, int TotalPaginas, int TotalCount)> GetByMotoristaIdAsync(
        Guid motoristaId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = context.Veiculos
            .Where(v => v.MotoristaId == motoristaId);

        var totalCount = await query.CountAsync(ct);
        var totalPaginas = (int)Math.Ceiling((double)totalCount / pageSize);
        var items = await query
            .OrderBy(v => v.Modelo)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalPaginas, totalCount);
    }

    public async Task AddAsync(Veiculo veiculo, CancellationToken ct = default)
        => await context.Veiculos.AddAsync(veiculo, ct);

    public async Task UpdateAsync(Veiculo veiculo, CancellationToken ct = default)
        => context.Veiculos.Update(veiculo);
}