using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;
using RotaCerta.Domain.Services.IEmail;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Infraestructure.Services.LembreteBackgroundService
{
    public class LembreteTrialBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LembreteTrialBackgroundService> _logger;
        private readonly TimeSpan _intervalo = TimeSpan.FromHours(24);
        private const int DiasAntesDoVencimento = 2;

        public LembreteTrialBackgroundService(IServiceScopeFactory scopeFactory, ILogger<LembreteTrialBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_intervalo);

            do
            {
                await ProcessarLembretesAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task ProcessarLembretesAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var whatsAppService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();
            var sendEmail = scope.ServiceProvider.GetRequiredService<ISendEmail>();

            await ProcessarLembretesTrialAsync(unitOfWork, whatsAppService, sendEmail, cancellationToken);
            await ProcessarLembretesRenovacaoAsync(unitOfWork, whatsAppService, sendEmail, cancellationToken);
        }

        private async Task ProcessarLembretesTrialAsync(
            IUnitOfWork unitOfWork, IWhatsAppService whatsAppService, ISendEmail sendEmail, CancellationToken cancellationToken)
        {
            var assinaturas = await unitOfWork.AssinaturaRepository
                .GetTrialsProximosDoFimAsync(DiasAntesDoVencimento, cancellationToken);

            foreach (var assinatura in assinaturas)
            {
                var motorista = await unitOfWork.MotoristaRepository.GetByIdAsync(assinatura.MotoristaId, cancellationToken);
                if (motorista is null)
                    continue;

                var diasRestantes = (assinatura.TrialFimEm.Date - DateTime.UtcNow.Date).Days;

                try
                {
                    await whatsAppService.EnviarLembreteTrialAsync(motorista.Telefone, motorista.Nome, diasRestantes, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar lembrete de trial por WhatsApp para a assinatura {AssinaturaId}", assinatura.Id);
                }

                try
                {
                    await sendEmail.LembreteTrialAsync(motorista, diasRestantes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar lembrete de trial por e-mail para a assinatura {AssinaturaId}", assinatura.Id);
                }

                assinatura.MarcarLembreteEnviado();
                await unitOfWork.AssinaturaRepository.UpdateAsync(assinatura, cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);
            }
        }

        private async Task ProcessarLembretesRenovacaoAsync(
            IUnitOfWork unitOfWork, IWhatsAppService whatsAppService, ISendEmail sendEmail, CancellationToken cancellationToken)
        {
            var assinaturas = await unitOfWork.AssinaturaRepository
                .GetAssinaturasProximasDaRenovacaoAsync(DiasAntesDoVencimento, cancellationToken);

            foreach (var assinatura in assinaturas)
            {
                var motorista = await unitOfWork.MotoristaRepository.GetByIdAsync(assinatura.MotoristaId, cancellationToken);
                if (motorista is null)
                    continue;

                var diasRestantes = (assinatura.ProximaCobrancaEm!.Value.Date - DateTime.UtcNow.Date).Days;

                try
                {
                    await whatsAppService.EnviarLembreteRenovacaoAsync(motorista.Telefone, motorista.Nome, diasRestantes, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar lembrete de renovação por WhatsApp para a assinatura {AssinaturaId}", assinatura.Id);
                }

                try
                {
                    await sendEmail.LembreteRenovacaoAsync(motorista, diasRestantes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar lembrete de renovação por e-mail para a assinatura {AssinaturaId}", assinatura.Id);
                }

                assinatura.MarcarLembreteEnviado();
                await unitOfWork.AssinaturaRepository.UpdateAsync(assinatura, cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);
            }
        }
    }
}
