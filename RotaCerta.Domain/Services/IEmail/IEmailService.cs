using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Domain.Services.IEmail
{
    public interface IEmailService
    {
        Task SendEmailService(string subject, string toEmail, string userName, string message, bool isHtml = false);
    }
}
