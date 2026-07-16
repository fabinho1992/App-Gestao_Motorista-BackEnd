using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Services;

namespace RotaCerta.Application.ManutencaoHandler.Commands.RegistrarManutencao;

public class RegistrarManutencaoHandler : IRequestHandler<RegistrarManutencaoCommand, ResultViewModel<ManutencaoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;

    public RegistrarManutencaoHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
    }

    public async Task<ResultViewModel<ManutencaoDto>> Handle(
        RegistrarManutencaoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel<ManutencaoDto>.Error("Usuário não autenticado.");

            var veiculo = await _unitOfWork.VeiculoRepository
                .GetByIdAsync(request.VeiculoId, cancellationToken);

            if (veiculo is null)
                return ResultViewModel<ManutencaoDto>.Error("Veículo não encontrado.");

            if (veiculo.MotoristaId != motoristaId)
                return ResultViewModel<ManutencaoDto>.Error("Veículo não pertence ao motorista autenticado.");

            var manutencao = Manutencao.Registrar(
                request.VeiculoId,
                request.Tipo,
                request.Descricao,
                request.DataRealizacao,
                request.KmRealizacao,
                request.Custo,
                request.Observacao);

            await _unitOfWork.ManutencaoRepository.AddAsync(manutencao, cancellationToken);

            // integração com o alerta de óleo existente — não altera a lógica, apenas aciona
            if (request.Tipo == TipoManutencao.Oleo)
            {
                veiculo.RegistrarTrocaOleo();
                await _unitOfWork.VeiculoRepository.UpdateAsync(veiculo, cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            var dto = new ManutencaoDto
            {
                Id = manutencao.Id,
                Tipo = manutencao.Tipo.ToString(),
                Descricao = manutencao.Descricao,
                DataRealizacao = manutencao.DataRealizacao,
                KmRealizacao = manutencao.KmRealizacao,
                Custo = manutencao.Custo,
                Observacao = manutencao.Observacao
            };

            return ResultViewModel<ManutencaoDto>.Success(dto);
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel<ManutencaoDto>.Error(ex.Message);
        }
    }
}
