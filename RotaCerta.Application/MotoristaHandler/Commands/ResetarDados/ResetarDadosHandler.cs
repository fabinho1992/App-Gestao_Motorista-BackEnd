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

            var viagens = await _context.Viagens
                .IgnoreQueryFilters()
                .Where(v => v.MotoristaId == motoristaId)
                .ToListAsync(cancellationToken);

            foreach (var viagem in viagens)
            {
                var entregas = await _context.Entregas
                    .Where(e => e.ViagemId == viagem.Id)
                    .ToListAsync(cancellationToken);

                _context.Entregas.RemoveRange(entregas);
            }

            _context.Viagens.RemoveRange(viagens);

            var veiculos = await _context.Veiculos
                .IgnoreQueryFilters()
                .Where(v => v.MotoristaId == motoristaId)
                .ToListAsync(cancellationToken);

            _context.Veiculos.RemoveRange(veiculos);

            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel.Success();
        }
        catch (Exception)
        {
            return ResultViewModel.Error("Erro ao resetar dados. Tente novamente.");
        }
    }
}
