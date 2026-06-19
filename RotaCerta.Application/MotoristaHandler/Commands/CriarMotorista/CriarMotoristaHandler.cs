using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.ModelsAuthentication;
using RotaCerta.Domain.Services.IAuthService;

namespace RotaCerta.Application.MotoristaHandler.Commands.CriarMotorista;

public class CriarMotoristaHandler : IRequestHandler<CriarMotoristaCommand, ResultViewModel<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICreateUser _createUser;

    public CriarMotoristaHandler(
        IUnitOfWork unitOfWork,
        ICreateUser createUser)
    {
        _unitOfWork = unitOfWork;
        _createUser = createUser;
    }

    public async Task<ResultViewModel<Guid>> Handle(
        CriarMotoristaCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var cpfExistente = await _unitOfWork.MotoristaRepository
                .GetByCpfAsync(request.Cpf, cancellationToken);

            if (cpfExistente is not null)
                return ResultViewModel<Guid>.Error("CPF já cadastrado.");

            var emailExistente = await _unitOfWork.MotoristaRepository
                .GetByEmailAsync(request.Email, cancellationToken);

            if (emailExistente is not null)
                return ResultViewModel<Guid>.Error("Email já cadastrado.");

            var motorista = Motorista.Criar(
                request.Nome,
                request.Cpf,
                request.Email,
                request.telefone,
                request.Cnh,
                request.vencimentoCnh);

            var registerUser = new RegisterUser(
                request.Nome,
                request.Email,
                request.senha,
                motorista.Id);

            var identityResult = await _createUser.CreateUserAsync(registerUser);

            if (identityResult.Status != "Ok")
                return ResultViewModel<Guid>.Error(identityResult.Message!);

            await _unitOfWork.MotoristaRepository.AddAsync(motorista, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel<Guid>.Success(motorista.Id);
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel<Guid>.Error(ex.Message);
        }
    }
}
