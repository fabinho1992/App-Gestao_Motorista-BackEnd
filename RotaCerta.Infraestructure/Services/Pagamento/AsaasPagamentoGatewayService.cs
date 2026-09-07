using Microsoft.Extensions.Configuration;
using RotaCerta.Domain.Services;
using RotaCerta.Domain.Services.Pagamento;
using RotaCerta.Domain.Services.Pagamento.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace RotaCerta.Infraestructure.Pagamento;

public class AsaasPagamentoGatewayService : IPagamentoGatewayService
{
    private readonly HttpClient _httpClient;

    public AsaasPagamentoGatewayService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        var baseUrl = configuration["Asaas:BaseUrl"]
            ?? throw new InvalidOperationException("Asaas:BaseUrl não configurada.");
        var apiKey = configuration["Asaas:ApiKey"]
            ?? throw new InvalidOperationException("Asaas:ApiKey não configurada.");

        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add("access_token", apiKey);
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RotaCerta.API", "1.0.0"));
    }

    public async Task<string> CriarClienteAsync(
        string nome, string cpf, string email, string telefone, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            name = nome,
            cpfCnpj = cpf.Replace(".", "").Replace("-", ""),
            email,
            mobilePhone = telefone
        };

        var response = await _httpClient.PostAsJsonAsync("/v3/customers", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Erro ao criar cliente no Asaas: {erro}");
        }

        var resultado = await response.Content.ReadFromJsonAsync<AsaasIdResponse>(cancellationToken: cancellationToken);
        return resultado!.Id;
    }

    public async Task<string> CriarAssinaturaAsync(
        string clienteId, decimal valor, string descricao, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            customer = clienteId,
            billingType = "PIX",
            nextDueDate = DateTime.UtcNow.Date.AddDays(1).ToString("yyyy-MM-dd"),
            value = valor,
            cycle = "MONTHLY",
            description = descricao
        };

        var response = await _httpClient.PostAsJsonAsync("/v3/subscriptions", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Erro ao criar assinatura no Asaas: {erro}");
        }

        var resultado = await response.Content.ReadFromJsonAsync<AsaasIdResponse>(cancellationToken: cancellationToken);
        return resultado!.Id;
    }

    public async Task<string> ObterPrimeiraCobrancaIdAsync(string assinaturaId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/v3/subscriptions/{assinaturaId}/payments", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Erro ao buscar cobranças da assinatura: {erro}");
        }

        var resultado = await response.Content.ReadFromJsonAsync<AsaasListaResponse>(cancellationToken: cancellationToken);
        var cobranca = resultado?.Data.FirstOrDefault()
            ?? throw new InvalidOperationException("Nenhuma cobrança encontrada para essa assinatura ainda.");

        return cobranca.Id;
    }

    public async Task<PixQrCodeDto> ObterQrCodePixAsync(string cobrancaId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/v3/payments/{cobrancaId}/pixQrCode", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Erro ao obter QR Code Pix: {erro}");
        }

        var resultado = await response.Content.ReadFromJsonAsync<AsaasQrCodeResponse>(cancellationToken: cancellationToken);
        return new PixQrCodeDto(resultado!.EncodedImage, resultado.Payload, resultado.ExpirationDate);
    }

    public async Task<string> ObterCobrancaEmAbertoAsync(string assinaturaId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/v3/subscriptions/{assinaturaId}/payments?status=PENDING", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Erro ao buscar cobrança em aberto da assinatura: {erro}");
        }

        var resultado = await response.Content.ReadFromJsonAsync<AsaasListaResponse>(cancellationToken: cancellationToken);
        var cobranca = resultado?.Data.FirstOrDefault()
            ?? throw new InvalidOperationException("Nenhuma cobrança em aberto encontrada para essa assinatura.");

        return cobranca.Id;
    }


    private record AsaasListaResponse([property: JsonPropertyName("data")] List<AsaasIdResponse> Data);
    private record AsaasIdResponse([property: JsonPropertyName("id")] string Id);

    private record AsaasQrCodeResponse(
    [property: JsonPropertyName("encodedImage")] string EncodedImage,
    [property: JsonPropertyName("payload")] string Payload,
    [property: JsonPropertyName("expirationDate")] string ExpirationDate);
}