namespace RotaCerta.Domain.Common;

public interface IImagemStorageService
{
    Task<string> UploadImagemAsync(
        Stream arquivo,
        string nomeArquivo,
        string contentType,
        CancellationToken cancellationToken = default);
}
