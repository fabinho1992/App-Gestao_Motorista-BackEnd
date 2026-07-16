using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.ManutencaoHandler.Commands.ExcluirManutencao;

public class ExcluirManutencaoHandler : IRequestHandler<ExcluirManutencaoCommand, ResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public ExcluirManutencaoHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel> Handle(
        ExcluirManutencaoCommand request,
        CancellationToken ct)
    {
        if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
            return ResultViewModel.Error("Não autenticado.");

        var manutencao = await _unitOfWork.ManutencaoRepository
            .GetByIdAsync(request.ManutencaoId, ct);

        if (manutencao is null)
            return ResultViewModel.Error("Manutenção não encontrada.");

        var veiculo = await _unitOfWork.VeiculoRepository
            .GetByIdAsync(manutencao.VeiculoId, ct);

        if (veiculo is null || veiculo.MotoristaId != motoristaId)
            return ResultViewModel.Error("Manutenção não pertence ao motorista.");

        // não deleta do banco — só marca como excluído
        manutencao.MarcarComoExcluido();

        await _unitOfWork.ManutencaoRepository.UpdateAsync(manutencao, ct);
        await _unitOfWork.CommitAsync(ct);

        return ResultViewModel.Success();
    }
}
