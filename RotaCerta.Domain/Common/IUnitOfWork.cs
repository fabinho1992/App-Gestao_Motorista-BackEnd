using RotaCerta.Domain.Common.Interfaces;

namespace RotaCerta.Domain.Common;

public interface IUnitOfWork
{
    IMotoristaRepository MotoristaRepository { get; }
    IVeiculoRepository VeiculoRepository { get; }
    IViagemRepository ViagemRepository { get; }
    IEntregaRepository EntregaRepository { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}