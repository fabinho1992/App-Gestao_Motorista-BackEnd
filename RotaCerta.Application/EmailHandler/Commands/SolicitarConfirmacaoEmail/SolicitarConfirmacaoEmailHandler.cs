using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;
using RotaCerta.Domain.Services.IEmail;
using RotaCerta.Infraestructure.Context.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.EmailHandler.Commands.ConfirmacaoEmail
{
    public class SolicitarConfirmacaoEmailHandler : IRequestHandler<SolicitarConfirmacaoEmailCommand, ResultViewModel<string>>
    {
        private readonly ISendEmail _sendEmail;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioContext _usuarioContext;

        public SolicitarConfirmacaoEmailHandler(
            ISendEmail sendEmail, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
        {
            _sendEmail = sendEmail;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _usuarioContext = usuarioContext;
        }

        public async Task<ResultViewModel<string>> Handle(SolicitarConfirmacaoEmailCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel<string>.Error("Usuário não autenticado.");

            var motorista = await _unitOfWork.MotoristaRepository.GetByIdAsync(motoristaId, cancellationToken);
            if (motorista is null)
                return ResultViewModel<string>.Error("Motorista não encontrado.");

            var userIdentity = await _userManager.Users.FirstOrDefaultAsync(u => u.MotoristaId == motoristaId, cancellationToken);
            if (userIdentity is null)
                return ResultViewModel<string>.Error("Usuário não encontrado.");

            if (userIdentity.EmailConfirmed)
                return ResultViewModel<string>.Error("E-mail já confirmado.");

            var code = Random.Shared.Next(100000, 999999).ToString();

            userIdentity.EmailConfirmationCode = code;
            userIdentity.EmailConfirmationCodeExpiration = DateTimeOffset.UtcNow.AddMinutes(10);

            await _userManager.UpdateAsync(userIdentity);

            await _sendEmail.EnviarConfirmacaoEmailAsync(motorista, code);

            return ResultViewModel<string>.Success("Código de confirmação enviado para seu e-mail.");
        }
    
    }
}
