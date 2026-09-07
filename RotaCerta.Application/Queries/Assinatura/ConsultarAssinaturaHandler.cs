using MediatR;
using RotaCerta.Application.Dtos;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Queries.Assinatura
{
    public class ConsultarAssinaturaHandler : IRequestHandler<ConsultarAssinaturaQuery, ResultViewModel<StatusAssinaturaViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioContext _usuarioContext;

        public ConsultarAssinaturaHandler(IUnitOfWork unitOfWork, IUsuarioContext usuarioContext)
        {
            _unitOfWork = unitOfWork;
            _usuarioContext = usuarioContext;
        }

        public async Task<ResultViewModel<StatusAssinaturaViewModel>> Handle(
            ConsultarAssinaturaQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel<StatusAssinaturaViewModel>.Error("Usuário não autenticado.");

            var assinatura = await _unitOfWork.AssinaturaRepository.GetByMotoristaIdAsync(motoristaId, cancellationToken);
            if (assinatura is null)
                return ResultViewModel<StatusAssinaturaViewModel>.Error("Assinatura não encontrada.");

            int? diasRestantes = assinatura.Status switch
            {
                StatusAssinatura.TrialAtivo => (assinatura.TrialFimEm.Date - DateTime.UtcNow.Date).Days,
                StatusAssinatura.Ativa when assinatura.ProximaCobrancaEm.HasValue =>
                    (assinatura.ProximaCobrancaEm.Value.Date - DateTime.UtcNow.Date).Days,
                _ => null
            };

            var viewModel = new StatusAssinaturaViewModel(
                assinatura.Status.ToString(),
                assinatura.TrialFimEm,
                assinatura.ProximaCobrancaEm,
                diasRestantes
            );

            return ResultViewModel<StatusAssinaturaViewModel>.Success(viewModel);
        }
    }
}
