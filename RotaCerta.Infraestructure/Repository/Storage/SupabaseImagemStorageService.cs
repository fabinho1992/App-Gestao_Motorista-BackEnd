// Infrastructure/Storage/SupabaseImagemStorageService.cs
using Microsoft.Extensions.Options;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services; // ← interface IImagemStorageService
using RotaCerta.Infrastructure.Repository.Storage;
using System.Net.Http.Headers;

namespace RotaCerta.Infraestructure.Storage;

public class SupabaseImagemStorageService : IImagemStorageService
{
    private readonly HttpClient _httpClient;
    private readonly SupabaseStorageOptions _options;

    public SupabaseImagemStorageService(
        HttpClient httpClient,
        IOptions<SupabaseStorageOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> UploadImagemAsync(
        Stream arquivo,
        string nomeArquivo,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Url))
            throw new InvalidOperationException("URL do Supabase Storage não configurada.");

        if (string.IsNullOrWhiteSpace(_options.Bucket))
            throw new InvalidOperationException("Bucket do Supabase Storage não configurado.");

        if (string.IsNullOrWhiteSpace(_options.ServiceRoleKey))
            throw new InvalidOperationException("ServiceRoleKey do Supabase não configurada.");

        // path organizado por motorista e entrega
        // ex: comprovantes/{motoristaId}/{entregaId}/guid.jpg
        var path = $"comprovantes/{nomeArquivo}";

        var uploadUrl = $"{_options.Url}/storage/v1/object/{_options.Bucket}/{path}";

        using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ServiceRoleKey);
        request.Headers.Add("apikey", _options.ServiceRoleKey);

        using var content = new StreamContent(arquivo);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        request.Content = content;

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Erro ao enviar imagem. Status: {response.StatusCode}. Erro: {erro}");
        }

        // retorna a URL pública da imagem
        return $"{_options.Url}/storage/v1/object/public/{_options.Bucket}/{path}";
    }
}