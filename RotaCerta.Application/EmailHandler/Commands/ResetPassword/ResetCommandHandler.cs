using MediatR;
using Microsoft.AspNetCore.Identity;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services.IEmail;
using RotaCerta.Infraestructure.Context.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.EmailHandler.Commands.ResetPassword
{
    public class ResetCommandHandler : IRequestHandler<ResetCommand, ResultViewModel<string>>
    {
        private readonly ISendEmail _sendEmail;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ResetCommandHandler(ISendEmail sendEmail, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _sendEmail = sendEmail;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultViewModel<string>> Handle(ResetCommand request, CancellationToken cancellationToken)
        {
            var motorista = await _unitOfWork.MotoristaRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (motorista is null)
                return ResultViewModel<string>.Error("E-mail não encontrado.");

            var userIdentity = await _userManager.FindByEmailAsync(request.Email);
            if (userIdentity is null)
                return ResultViewModel<string>.Error("E-mail não encontrado.");

            var code = Random.Shared.Next(100000, 999999).ToString();

            userIdentity.ResetToken = code;
            userIdentity.ResetTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(10);

            await _userManager.UpdateAsync(userIdentity);

            await _sendEmail.ResetPassword(motorista, code);

            return ResultViewModel<string>.Success("Código de recuperação enviado para seu e-mail.");
        }
    }
}
