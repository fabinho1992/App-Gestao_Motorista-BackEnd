using MediatR;
using Microsoft.EntityFrameworkCore;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;
using RotaCerta.Infraestructure.Context;

namespace RotaCerta.Application.MotoristaHandler.Commands.ResetarDados;

public class ResetarDadosHandler : IRequestHandler<ResetarDadosCommand, ResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;
    private readonly DbRotaCertaContext _context;

    public ResetarDadosHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext, DbRotaCertaContext context)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
        _context = context;
    }

    public async Task<ResultViewModel> Handle(
        ResetarDadosCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel.Error("Usuário não autenticado.");

            var entregaIds = await _context.Viagens
                .Where(v => v.MotoristaId == motoristaId)
                .Select(v => v.Id)
                .ToListAsync(cancellationToken);

            var entregas = await _context.Entregas
                .Where(e => entregaIds.Contains(e.ViagemId))
                .ToListAsync(cancellationToken);

            _context.Entregas.RemoveRange(entregas);
            await _context.SaveChangesAsync(cancellationToken);

            var viagens = await _context.Viagens
                .IgnoreQueryFilters()
                .Where(v => v.MotoristaId == motoristaId)
                .ToListAsync(cancellationToken);

            _context.Viagens.RemoveRange(viagens);
            await _context.SaveChangesAsync(cancellationToken);

            var veiculos = await _context.Veiculos
                .IgnoreQueryFilters()
                .Where(v => v.MotoristaId == motoristaId)
                .ToListAsync(cancellationToken);

            _context.Veiculos.RemoveRange(veiculos);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultViewModel.Success();
        }
        catch (Exception)
        {
            return ResultViewModel.Error("Erro ao resetar dados. Tente novamente.");
        }
    }
}
