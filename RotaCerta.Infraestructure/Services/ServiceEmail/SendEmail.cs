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
                <!DOCTYPE html>
                <html lang=""pt-BR"">
                <head>
                <meta charset=""utf-8"" />
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                </head>
                <body style=""margin:0; padding:0; background-color:#f5f5f7; font-family: 'Segoe UI', Arial, sans-serif;"">
                  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f5f5f7; padding: 32px 0;"">
                    <tr>
                      <td align=""center"">
                        <table role=""presentation"" width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius: 12px; overflow:hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.06);"">
                          <tr>
                            <td style=""background-color:#534AB7; padding: 28px 32px; text-align:center;"">
                              <span style=""color:#ffffff; font-size: 20px; font-weight:600;"">Rota Certa</span>
                            </td>
                          </tr>
                          <tr>
                            <td style=""padding: 32px;"">
                              <p style=""margin:0 0 16px; font-size:16px; color:#111827;"">Olá, {motorista.Nome}</p>
                              <p style=""margin:0 0 24px; font-size:15px; color:#374151; line-height:1.6;"">
                                Recebemos uma solicitação para redefinir sua senha no <strong>Rota Certa</strong>. Use o código abaixo para continuar:
                              </p>
                              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin: 0 0 24px;"">
                                <tr>
                                  <td style=""background-color:#f5f3ff; border:1px solid #ddd6fe; border-radius:10px; padding:20px; text-align:center;"">
                                    <span style=""font-size:32px; font-weight:700; letter-spacing:8px; color:#534AB7;"">{code}</span>
                                  </td>
                                </tr>
                              </table>
                              <p style=""margin:0 0 8px; font-size:13px; color:#6b7280;"">
                                Este código expira em 10 minutos.
                              </p>
                              <p style=""margin:0; font-size:13px; color:#6b7280;"">
                                Se você não solicitou a redefinição de senha, ignore este e-mail.
                              </p>
                            </td>
                          </tr>
                          <tr>
                            <td style=""background-color:#f9fafb; padding: 20px 32px; text-align:center; border-top:1px solid #e5e7eb;"">
                              <p style=""margin:0; font-size:12px; color:#9ca3af;"">
                                Rota Certa &middot; Gestão financeira para motoristas autônomos
                              </p>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>
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
                <!DOCTYPE html>
                <html lang=""pt-BR"">
                <head>
                <meta charset=""utf-8"" />
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                </head>
                <body style=""margin:0; padding:0; background-color:#f5f5f7; font-family: 'Segoe UI', Arial, sans-serif;"">
                  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f5f5f7; padding: 32px 0;"">
                    <tr>
                      <td align=""center"">
                        <table role=""presentation"" width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius: 12px; overflow:hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.06);"">
                          <tr>
                            <td style=""background-color:#534AB7; padding: 28px 32px; text-align:center;"">
                              <span style=""color:#ffffff; font-size: 20px; font-weight:600;"">Rota Certa</span>
                            </td>
                          </tr>
                          <tr>
                            <td style=""padding: 32px;"">
                              <p style=""margin:0 0 16px; font-size:16px; color:#111827;"">Olá, {motorista.Nome}</p>
                              <p style=""margin:0 0 24px; font-size:15px; color:#374151; line-height:1.6;"">
                                Seu período de teste gratuito no <strong>Rota Certa</strong> está terminando em <strong>{diasRestantes} dia(s)</strong>.
                                Para continuar usando o app sem interrupções, assine agora mesmo direto pelo aplicativo.
                              </p>
                              <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" style=""margin: 0 auto 24px;"">
                                <tr>
                                  <td style=""background-color:#534AB7; border-radius:8px;"">
                                    <a href=""https://approtacerta.com.br/perfil"" style=""display:inline-block; padding: 14px 32px; color:#ffffff; font-size:15px; font-weight:600; text-decoration:none;"">
                                      Assinar agora
                                    </a>
                                  </td>
                                </tr>
                              </table>
                              <p style=""margin:0; font-size:13px; color:#6b7280; text-align:center;"">
                                Abra o app e vá em ""Meu Perfil"" para gerar o QR Code Pix.
                              </p>
                            </td>
                          </tr>
                          <tr>
                            <td style=""background-color:#f9fafb; padding: 20px 32px; text-align:center; border-top:1px solid #e5e7eb;"">
                              <p style=""margin:0; font-size:12px; color:#9ca3af;"">
                                Rota Certa &middot; Gestão financeira para motoristas autônomos
                              </p>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>
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
                <!DOCTYPE html>
                <html lang=""pt-BR"">
                <head>
                <meta charset=""utf-8"" />
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                </head>
                <body style=""margin:0; padding:0; background-color:#f5f5f7; font-family: 'Segoe UI', Arial, sans-serif;"">
                  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f5f5f7; padding: 32px 0;"">
                    <tr>
                      <td align=""center"">
                        <table role=""presentation"" width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius: 12px; overflow:hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.06);"">
                          <tr>
                            <td style=""background-color:#534AB7; padding: 28px 32px; text-align:center;"">
                              <span style=""color:#ffffff; font-size: 20px; font-weight:600;"">Rota Certa</span>
                            </td>
                          </tr>
                          <tr>
                            <td style=""padding: 32px;"">
                              <p style=""margin:0 0 16px; font-size:16px; color:#111827;"">Olá, {motorista.Nome}</p>
                              <p style=""margin:0 0 24px; font-size:15px; color:#374151; line-height:1.6;"">
                                Sua assinatura do <strong>Rota Certa</strong> vence em <strong>{diasRestantes} dia(s)</strong>.
                                Para continuar usando o app sem interrupções, renove agora mesmo direto pelo aplicativo.
                              </p>
                              <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" style=""margin: 0 auto 24px;"">
                                <tr>
                                  <td style=""background-color:#534AB7; border-radius:8px;"">
                                    <a href=""https://approtacerta.com.br/perfil"" style=""display:inline-block; padding: 14px 32px; color:#ffffff; font-size:15px; font-weight:600; text-decoration:none;"">
                                      Renovar assinatura
                                    </a>
                                  </td>
                                </tr>
                              </table>
                              <p style=""margin:0; font-size:13px; color:#6b7280; text-align:center;"">
                                Abra o app e vá em ""Meu Perfil"" para gerar o QR Code Pix.
                              </p>
                            </td>
                          </tr>
                          <tr>
                            <td style=""background-color:#f9fafb; padding: 20px 32px; text-align:center; border-top:1px solid #e5e7eb;"">
                              <p style=""margin:0; font-size:12px; color:#9ca3af;"">
                                Rota Certa &middot; Gestão financeira para motoristas autônomos
                              </p>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>
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

        public async Task EnviarConfirmacaoEmailAsync(Motorista motorista, string code)
        {
            var message = $@"
                <!DOCTYPE html>
                <html lang=""pt-BR"">
                <head>
                <meta charset=""utf-8"" />
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                </head>
                <body style=""margin:0; padding:0; background-color:#f5f5f7; font-family: 'Segoe UI', Arial, sans-serif;"">
                  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f5f5f7; padding: 32px 0;"">
                    <tr>
                      <td align=""center"">
                        <table role=""presentation"" width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius: 12px; overflow:hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.06);"">
                          <tr>
                            <td style=""background-color:#534AB7; padding: 28px 32px; text-align:center;"">
                              <span style=""color:#ffffff; font-size: 20px; font-weight:600;"">Rota Certa</span>
                            </td>
                          </tr>
                          <tr>
                            <td style=""padding: 32px;"">
                              <p style=""margin:0 0 16px; font-size:16px; color:#111827;"">Olá, {motorista.Nome}</p>
                              <p style=""margin:0 0 24px; font-size:15px; color:#374151; line-height:1.6;"">
                                Confirme seu e-mail no <strong>Rota Certa</strong> usando o código abaixo:
                              </p>
                              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin: 0 0 24px;"">
                                <tr>
                                  <td style=""background-color:#f5f3ff; border:1px solid #ddd6fe; border-radius:10px; padding:20px; text-align:center;"">
                                    <span style=""font-size:32px; font-weight:700; letter-spacing:8px; color:#534AB7;"">{code}</span>
                                  </td>
                                </tr>
                              </table>
                              <p style=""margin:0; font-size:13px; color:#6b7280;"">
                                Este código expira em 10 minutos.
                              </p>
                            </td>
                          </tr>
                          <tr>
                            <td style=""background-color:#f9fafb; padding: 20px 32px; text-align:center; border-top:1px solid #e5e7eb;"">
                              <p style=""margin:0; font-size:12px; color:#9ca3af;"">
                                Rota Certa &middot; Gestão financeira para motoristas autônomos
                              </p>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>
                ";

            await _emailService.SendEmailService("Confirme seu e-mail", motorista.Email, motorista.Nome, message, true);

            _logger.LogInformation("E-mail de confirmação enviado para {Email}", motorista.Email);
        }
    }
}
