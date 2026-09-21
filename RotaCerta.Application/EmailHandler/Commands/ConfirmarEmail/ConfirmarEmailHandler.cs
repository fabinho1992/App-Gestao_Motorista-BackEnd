using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Services;
using RotaCerta.Infraestructure.Context.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.EmailHandler.Commands.ConfirmarEmail
{
    public class ConfirmarEmailHandler : IRequestHandler<ConfirmarEmailCommand, ResultViewModel<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsuarioContext _usuarioContext;

        public ConfirmarEmailHandler(UserManager<ApplicationUser> userManager, IUsuarioContext usuarioContext)
        {
            _userManager = userManager;
            _usuarioContext = usuarioContext;
        }

        public async Task<ResultViewModel<string>> Handle(ConfirmarEmailCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel<string>.Error("Usuário não autenticado.");

            var userIdentity = await _userManager.Users.FirstOrDefaultAsync(u => u.MotoristaId == motoristaId, cancellationToken);
            if (userIdentity is null)
                return ResultViewModel<string>.Error("Usuário não encontrado.");

            if (userIdentity.EmailConfirmed)
                return ResultViewModel<string>.Success("E-mail já estava confirmado.");

            if (userIdentity.EmailConfirmationCode != request.Code)
                return ResultViewModel<string>.Error("Código inválido.");

            if (userIdentity.EmailConfirmationCodeExpiration < DateTimeOffset.UtcNow)
                return ResultViewModel<string>.Error("Código expirado.");

            userIdentity.EmailConfirmed = true;
            userIdentity.EmailConfirmationCode = string.Empty;

            await _userManager.UpdateAsync(userIdentity);

            return ResultViewModel<string>.Success("E-mail confirmado com sucesso!");
        }
    }
}
