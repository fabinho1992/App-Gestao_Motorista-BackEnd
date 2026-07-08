using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Viagens;

namespace RotaCerta.Domain.Common.Interfaces;

public interface IMotoristaRepository
{
    Task<Motorista?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Motorista?> GetByCpfAsync(string cpf, CancellationToken ct = default);
    Task<Motorista?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(Motorista motorista, CancellationToken ct = default);
    Task UpdateAsync(Motorista motorista, CancellationToken ct = default);
}

public interface IVeiculoRepository
{
    Task<Veiculo?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(List<Veiculo> Items, int TotalPaginas, int TotalCount)> GetByMotoristaIdAsync(Guid motoristaId, int pageNumber, int pageSize, CancellationToken ct = default);
    Task AddAsync(Veiculo veiculo, CancellationToken ct = default);
    Task UpdateAsync(Veiculo veiculo, CancellationToken ct = default);
}

public interface IViagemRepository
{
    Task<Viagem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(List<Viagem> Items, int TotalPaginas, int TotalCount)> GetByMotoristaIdAsync(Guid motoristaId, StatusViagem? status, DateOnly? dataInicio, DateOnly? dataFim, string? empresaContratante, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<List<Viagem>> GetByMotoristaIdMesAsync(Guid motoristaId, int mes, int ano, CancellationToken ct = default);
    Task<List<Viagem>> GetViagensEncerradasPorMesAsync(Guid motoristaId, int mes, int ano, CancellationToken ct = default);
    Task<List<string>> GetEmpresasDistintasAsync(Guid motoristaId, CancellationToken ct = default);
    Task<List<Viagem>> GetViagensAtivasPorMotoristaAsync(Guid motoristaId, CancellationToken ct = default);
    Task AddAsync(Viagem viagem, CancellationToken ct = default);
    Task UpdateAsync(Viagem viagem, CancellationToken ct = default);
}

public interface IEntregaRepository
{
    Task<Entrega?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(List<Entrega> Items, int TotalPaginas, int TotalCount)> GetByViagemIdAsync(Guid viagemId, int pageNumber, int pageSize, CancellationToken ct = default);
    Task AddAsync(Entrega entrega, CancellationToken ct = default);
    Task UpdateAsync(Entrega entrega, CancellationToken ct = default);
}