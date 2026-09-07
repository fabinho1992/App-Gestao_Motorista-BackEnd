using Microsoft.Extensions.Configuration;
using Resend;
using RotaCerta.Domain.Services.IEmail;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace RotaCerta.Infraestructure.Services.ServiceEmail
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly IConfiguration _config;

        public EmailService(IResend resend, IConfiguration config)
        {
            _resend = resend;
            _config = config;
        }

        public async Task SendEmailService(string subject, string toEmail, string userName, string message, bool isHtml = false)
        {
            var fromEmail = _config["Resend:FromEmail"];

            var emailMessage = new EmailMessage
            {
                From = $"RotaCerta <{fromEmail}>",
                Subject = subject
            };
            emailMessage.To.Add($"{userName} <{toEmail}>");

            if (isHtml)
                emailMessage.HtmlBody = message;
            else
                emailMessage.TextBody = message;

            var response = await _resend.EmailSendAsync(emailMessage);
            Console.WriteLine(response.Content);
        }
    }
}
