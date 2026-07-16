using RotaCerta.Domain.Common;
using RotaCerta.Domain.Common.Interfaces;
using RotaCerta.Infraestructure.Context;
using RotaCerta.Infraestructure.DomainEvents;
using RotaCerta.Infrastructure.Repositories;

namespace RotaCerta.Infraestructure.Repository;

public sealed class UnitOfWork(
    DbRotaCertaContext context,
    DomainEventDispatcher dispatcher) : IUnitOfWork
{
    private IMotoristaRepository? _motoristaRepository;
    private IVeiculoRepository? _veiculoRepository;
    private IViagemRepository? _viagemRepository;
    private IEntregaRepository? _entregaRepository;
    private IManutencaoRepository? _manutencaoRepository;

    public IMotoristaRepository MotoristaRepository
        => _motoristaRepository ??= new MotoristaRepository(context);

    public IVeiculoRepository VeiculoRepository
        => _veiculoRepository ??= new VeiculoRepository(context);

    public IViagemRepository ViagemRepository
        => _viagemRepository ??= new ViagemRepository(context);

    public IEntregaRepository EntregaRepository
        => _entregaRepository ??= new EntregaRepository(context);

    public IManutencaoRepository ManutencaoRepository
        => _manutencaoRepository ??= new ManutencaoRepository(context);

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var entidades = context.ChangeTracker
            .Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        var eventos = entidades
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entidades.ForEach(e => e.ClearDomainEvents());

        var result = await context.SaveChangesAsync(cancellationToken);

        await dispatcher.DispatchAsync(eventos, cancellationToken);

        return result;
    }
}
