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
            if (string.IsNullOrWhiteSpace(request.Nome))
                return ResultViewModel<Guid>.Error(
                    "Nome é obrigatório.");

            var cpfNumeros = request.Cpf?.Replace(".", "").Replace("-", "") ?? "";
            if (string.IsNullOrWhiteSpace(cpfNumeros) || cpfNumeros.Length != 11)
                return ResultViewModel<Guid>.Error(
                    "CPF inválido. Informe os 11 dígitos do CPF.");

            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                return ResultViewModel<Guid>.Error(
                    "Email inválido.");

            if (string.IsNullOrWhiteSpace(request.Cnh))
                return ResultViewModel<Guid>.Error(
                    "CNH é obrigatória.");

            if (request.vencimentoCnh < DateOnly.FromDateTime(DateTime.Today))
                return ResultViewModel<Guid>.Error(
                    "CNH vencida. Informe uma CNH com validade futura.");

            if (string.IsNullOrWhiteSpace(request.telefone))
                return ResultViewModel<Guid>.Error(
                    "Telefone é obrigatório.");

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
