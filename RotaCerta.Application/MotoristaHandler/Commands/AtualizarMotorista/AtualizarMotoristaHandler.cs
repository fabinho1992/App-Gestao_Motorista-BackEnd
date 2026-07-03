using MediatR;
using Microsoft.AspNetCore.Identity;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;
using RotaCerta.Infraestructure.Context.Identity;

namespace RotaCerta.Application.MotoristaHandler.Commands.AtualizarMotorista;

public class AtualizarMotoristaHandler : IRequestHandler<AtualizarMotoristaCommand, ResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AtualizarMotoristaHandler(
        IUnitOfWork unitOfWork,
        IUsuarioContext usuarioContext,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
        _userManager = userManager;
    }

    public async Task<ResultViewModel> Handle(
        AtualizarMotoristaCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel.Error("Usuário não autenticado");

            var motorista = await _unitOfWork.MotoristaRepository
                .GetByIdAsync(motoristaId, cancellationToken);

            if (motorista is null)
                return ResultViewModel.Error("Motorista não encontrado");

            var cpfExistente = await _unitOfWork.MotoristaRepository
                .GetByCpfAsync(request.Cpf, cancellationToken);

            if (cpfExistente is not null && cpfExistente.Id != motoristaId)
                return ResultViewModel.Error("CPF já cadastrado por outro motorista");

            var emailExistente = await _unitOfWork.MotoristaRepository
                .GetByEmailAsync(request.Email, cancellationToken);

            if (emailExistente is not null && emailExistente.Id != motoristaId)
                return ResultViewModel.Error("Email já cadastrado por outro motorista");

            motorista.Atualizar(
                request.Nome,
                request.Cpf,
                request.Email,
                request.Telefone,
                request.Cnh,
                request.VencimentoCnh);

            await _unitOfWork.MotoristaRepository.UpdateAsync(motorista, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);


            var usuario = await _userManager.FindByIdAsync(
               _usuarioContext.MotoristaId ?? motoristaId.ToString());

            if (usuario is not null)
            {
                usuario.DisplayName = request.Nome;
                usuario.Email = request.Email;
                usuario.UserName = request.Email;
                usuario.NormalizedEmail = request.Email.ToUpperInvariant();
                usuario.NormalizedUserName = request.Email.ToUpperInvariant();

                await _userManager.UpdateAsync(usuario);
            }


            return ResultViewModel.Success();
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel.Error(ex.Message);
        }
    }
}
