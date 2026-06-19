using Microsoft.EntityFrameworkCore;
using RotaCerta.Domain.Common.Interfaces;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Viagens;
using RotaCerta.Infraestructure.Context;

namespace RotaCerta.Infrastructure.Repositories;

public sealed class ViagemRepository(DbRotaCertaContext context) : IViagemRepository
{
    public async Task<Viagem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Viagens
            .Include(v => v.Entregas)
            .Include(v => v.Veiculo)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<(List<Viagem> Items, int TotalPaginas, int TotalCount)> GetByMotoristaIdAsync(
        Guid motoristaId, StatusViagem? status, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = context.Viagens
            .Include(v => v.Entregas)
            .Where(v => v.MotoristaId == motoristaId);

        if (status.HasValue)
            query = query.Where(v => v.Status == status.Value);

        var totalCount = await query.CountAsync(ct);
        var totalPaginas = (int)Math.Ceiling((double)totalCount / pageSize);
        var items = await query
            .OrderByDescending(v => v.DataSaida)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalPaginas, totalCount);
    }

    public async Task<List<Viagem>> GetByMotoristaIdMesAsync(
        Guid motoristaId, int mes, int ano, CancellationToken ct = default)
        => await context.Viagens
            .Include(v => v.Entregas)
            .Where(v =>
                v.MotoristaId == motoristaId &&
                v.DataSaida.Month == mes &&
                v.DataSaida.Year == ano)
            .ToListAsync(ct);

    public async Task AddAsync(Viagem viagem, CancellationToken ct = default)
        => await context.Viagens.AddAsync(viagem, ct);

    public async Task UpdateAsync(Viagem viagem, CancellationToken ct = default)
        => context.Viagens.Update(viagem);
}
