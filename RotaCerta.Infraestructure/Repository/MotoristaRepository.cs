using Microsoft.EntityFrameworkCore;
using RotaCerta.Domain.Common.Interfaces;
using RotaCerta.Domain.Models;
using RotaCerta.Infraestructure.Context;

namespace RotaCerta.Infrastructure.Repositories;

public sealed class MotoristaRepository(DbRotaCertaContext context) : IMotoristaRepository
{
    public async Task<Motorista?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Motoristas
            .Include(m => m.Veiculos)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task AddAsync(Motorista motorista, CancellationToken ct = default)
        => await context.Motoristas.AddAsync(motorista, ct);

    public async Task UpdateAsync(Motorista motorista, CancellationToken ct = default)
        => context.Motoristas.Update(motorista);

    public async Task<Motorista?> GetByCpfAsync(string cpf, CancellationToken ct = default)
         => await context.Motoristas
            .Include(m => m.Veiculos)
            .FirstOrDefaultAsync(m => m.Cpf == cpf, ct);

    public async Task<Motorista?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await context.Motoristas
            .FirstOrDefaultAsync(m => m.Email == email, ct);
}