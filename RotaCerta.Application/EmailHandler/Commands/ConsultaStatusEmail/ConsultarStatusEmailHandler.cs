using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Services;
using RotaCerta.Infraestructure.Context.Identity;

namespace RotaCerta.Application.EmailHandler.Commands.ConsultaStatusEmail
{
    public class ConsultarStatusEmailHandler : IRequestHandler<ConsultarStatusEmailQuery, ResultViewModel<StatusEmailViewModel>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsuarioContext _usuarioContext;

        public ConsultarStatusEmailHandler(UserManager<ApplicationUser> userManager, IUsuarioContext usuarioContext)
        {
            _userManager = userManager;
            _usuarioContext = usuarioContext;
        }

        public async Task<ResultViewModel<StatusEmailViewModel>> Handle(ConsultarStatusEmailQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel<StatusEmailViewModel>.Error("Usuário não autenticado.");

            var userIdentity = await _userManager.Users.FirstOrDefaultAsync(u => u.MotoristaId == motoristaId, cancellationToken);
            if (userIdentity is null)
                return ResultViewModel<StatusEmailViewModel>.Error("Usuário não encontrado.");

            return ResultViewModel<StatusEmailViewModel>.Success(new StatusEmailViewModel(userIdentity.EmailConfirmed));
        }
    }
}
