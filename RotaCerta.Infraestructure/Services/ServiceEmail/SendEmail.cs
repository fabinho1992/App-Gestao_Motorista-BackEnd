using Microsoft.Extensions.Logging;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Services.IEmail;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Infraestructure.Services.ServiceEmail
{
    public class SendEmail : ISendEmail
    {
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SendEmail> _logger;

        public SendEmail(IEmailService emailService, IUnitOfWork unitOfWork, ILogger<SendEmail> logger)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ResetPassword(Motorista motorista, string code)
        {
            var message = $@"
                    Olá {motorista.Nome},

            Recebemos uma solicitação para redefinir sua senha no <strong>RotaCerta</strong>.

            Seu código de recuperação é:

            <h2 style=""letter-spacing:4px"">{code}</h2>

            Este código expira em 10 minutos.

            Se você não solicitou a redefinição de senha, ignore este email.

            Atenciosamente,  
            Equipe RotaCerta
            ";

            await _emailService.SendEmailService(
                "Redefinição de Senha",
                motorista.Email,
                motorista.Nome,
                message,
                true
            );

            _logger.LogInformation("E-mail de redefinição de senha enviado para {Email}", motorista.Email);
        }

        public async Task LembreteTrialAsync(Motorista motorista, int diasRestantes)
        {
            var message = $@"
                Olá {motorista.Nome},

            Seu período de teste gratuito no <strong>RotaCerta</strong> está terminando em {diasRestantes} dia(s).

            Para continuar usando o app sem interrupções, assine agora mesmo pelo aplicativo.

            Atenciosamente,
            Equipe RotaCerta
            ";

            await _emailService.SendEmailService(
                "Seu teste gratuito está acabando",
                motorista.Email,
                motorista.Nome,
                message,
                true
            );

            _logger.LogInformation("E-mail de lembrete de trial enviado para {Email}", motorista.Email);
        }

        public async Task LembreteRenovacaoAsync(Motorista motorista, int diasRestantes)
        {
            var message = $@"
                Olá {motorista.Nome},

            Sua assinatura do <strong>RotaCerta</strong> vence em {diasRestantes} dia(s).

            Acesse seu perfil no aplicativo para gerar o QR Code Pix e renovar sua assinatura.

            Atenciosamente,
            Equipe RotaCerta
            ";

            await _emailService.SendEmailService(
                "Sua assinatura está vencendo",
                motorista.Email,
                motorista.Nome,
                message,
                true
            );

            _logger.LogInformation("E-mail de lembrete de renovação enviado para {Email}", motorista.Email);
        }
    }
}
