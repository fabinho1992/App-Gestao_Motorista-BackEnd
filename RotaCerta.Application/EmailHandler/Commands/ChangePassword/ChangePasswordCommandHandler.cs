using MediatR;
using Microsoft.AspNetCore.Identity;
using RotaCerta.Application.Dtos;
using RotaCerta.Infraestructure.Context.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.EmailHandler.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResultViewModel<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResultViewModel<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return ResultViewModel<string>.Error("E-mail não encontrado.");

            if (user.ResetToken != request.Code)
                return ResultViewModel<string>.Error("Código inválido.");

            if (user.ResetTokenExpiration < DateTimeOffset.UtcNow)
                return ResultViewModel<string>.Error("Código expirado.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, request.Password);

            if (!result.Succeeded)
            {
                var erros = string.Join(" ", result.Errors.Select(e => e.Description));
                return ResultViewModel<string>.Error($"Falha ao alterar senha: {erros}");
            }

            user.ResetToken = string.Empty;
            await _userManager.UpdateAsync(user);

            return ResultViewModel<string>.Success("Senha alterada com sucesso!");
        }
    }
}
