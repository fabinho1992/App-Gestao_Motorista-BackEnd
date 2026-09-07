using Microsoft.Extensions.Configuration;
using RotaCerta.Domain.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace RotaCerta.Infraestructure.Services.WhatsApp
{
    public class WhatsAppMetaService : IWhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly string _phoneNumberId;
        private readonly string _templateName;
        private readonly string _templateRenovacaoName;

        public WhatsAppMetaService(HttpClient httpClient, IConfiguration configuration)
        {
            var accessToken = configuration["WhatsApp:AccessToken"];
            _phoneNumberId = configuration["WhatsApp:PhoneNumberId"]
                ?? throw new InvalidOperationException("WhatsApp:PhoneNumberId não configurado.");
            _templateName = configuration["WhatsApp:TemplateName"] ?? "lembrete_trial";
            _templateRenovacaoName = configuration["WhatsApp:TemplateRenovacaoName"] ?? "hello_world";

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/v21.0/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task EnviarLembreteTrialAsync(string numeroTelefone, string nomeMotorista, int diasRestantes, CancellationToken cancellationToken = default)
        {
            var numeroFormatado = FormatarNumero(numeroTelefone);

            var payload = new
            {
                messaging_product = "whatsapp",
                to = numeroFormatado,
                type = "template",
                template = new
                {
                    name = _templateName,
                    language = new { code = "pt_BR" },
                    components = new[]
                    {
                    new
                    {
                        type = "body",
                        parameters = new object[]
                        {
                            new { type = "text", text = nomeMotorista },
                            new { type = "text", text = diasRestantes.ToString() }
                        }
                    }
                }
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{_phoneNumberId}/messages", payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Erro ao enviar lembrete via WhatsApp: {erro}");
            }
        }

        public async Task EnviarLembreteRenovacaoAsync(string numeroTelefone, string nomeMotorista, int diasRestantes, CancellationToken cancellationToken = default)
        {
            var numeroFormatado = FormatarNumero(numeroTelefone);

            var payload = new
            {
                messaging_product = "whatsapp",
                to = numeroFormatado,
                type = "template",
                template = new
                {
                    name = _templateRenovacaoName,
                    language = new { code = "pt_BR" },
                    components = new[]
                    {
                new
                {
                    type = "body",
                    parameters = new object[]
                    {
                        new { type = "text", text = nomeMotorista },
                        new { type = "text", text = diasRestantes.ToString() }
                    }
                }
            }
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{_phoneNumberId}/messages", payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Erro ao enviar lembrete de renovação via WhatsApp: {erro}");
            }
        }

        private static string FormatarNumero(string numero)
        {
            // A Meta espera o número em formato E.164 sem o "+" (ex.: 5511999999999)
            return numero.Replace("+", "").Replace(" ", "").Replace("-", "");
        }
    }
}
