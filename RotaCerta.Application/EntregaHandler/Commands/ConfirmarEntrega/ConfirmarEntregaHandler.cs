using MediatR;
using Microsoft.AspNetCore.Http;
using RotaCerta.Application.Dtos;
using RotaCerta.Application.EntregaHandler.Commands.ConfirmarEntrega;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Services;

public class ConfirmarEntregaHandler : IRequestHandler<ConfirmarEntregaCommand, ResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IImagemStorageService _imagemStorageService;

    public ConfirmarEntregaHandler(
        IUnitOfWork unitOfWork,
        IUsuarioContext usuarioContext,
        IImagemStorageService imagemStorageService)
    {
        _unitOfWork = unitOfWork;
        _usuarioContext = usuarioContext;
        _imagemStorageService = imagemStorageService;
    }

    public async Task<ResultViewModel> Handle(
        ConfirmarEntregaCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Fotos recebidas: {request.Fotos?.Count ?? 0}");
            if (request.Fotos != null)
            {
                foreach (var foto in request.Fotos)
                {
                    Console.WriteLine($"Foto: {foto.FileName} | {foto.ContentType} | {foto.Length} bytes");
                }
            }

            if (!Guid.TryParse(_usuarioContext.MotoristaId, out var motoristaId))
                return ResultViewModel.Error("Usuário não autenticado.");

            var entrega = await _unitOfWork.EntregaRepository
                .GetByIdAsync(request.EntregaId, cancellationToken);

            if (entrega is null)
                return ResultViewModel.Error("Entrega não encontrada.");

            var viagem = await _unitOfWork.ViagemRepository
                .GetByIdAsync(entrega.ViagemId, cancellationToken);

            if (viagem is null || viagem.MotoristaId != motoristaId)
                return ResultViewModel.Error("Entrega não pertence ao motorista autenticado.");

            // faz upload de cada foto e adiciona na entrega
            if (request.Fotos is not null && request.Fotos.Count > 0)
            {
                foreach (var foto in request.Fotos)
                {
                    // valida cada foto
                    var validacao = ValidarImagem(foto);
                    if (!validacao.IsValida)
                        return ResultViewModel.Error(validacao.Mensagem);

                    // gera nome único para cada foto
                    var extensao = Path.GetExtension(foto.FileName);
                    var nomeArquivo = $"{motoristaId}/{entrega.Id}/{Guid.NewGuid()}{extensao}";

                    // faz upload
                    await using var stream = foto.OpenReadStream();
                    var fotoUrl = await _imagemStorageService.UploadImagemAsync(
                        stream,
                        nomeArquivo,
                        foto.ContentType,
                        cancellationToken);

                    // adiciona a URL na lista da entrega
                    entrega.AdicionarFoto(fotoUrl);
                }
            }

            // confirma a entrega — levanta EntregaConcluidaEvent
            entrega.ConfirmarEntrega();

            await _unitOfWork.EntregaRepository.UpdateAsync(entrega, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResultViewModel.Success();
        }
        catch (InvalidOperationException ex)
        {
            return ResultViewModel.Error(ex.Message);
        }
        catch (Exception ex)  // ← adiciona esse
        {
            return ResultViewModel.Error($"Erro inesperado: {ex.Message}");
        }
    }

    private static (bool IsValida, string Mensagem) ValidarImagem(IFormFile imagem)
    {
        var tiposPermitidos = new[] { "image/jpeg", "image/png", "image/webp" };

        if (!tiposPermitidos.Contains(imagem.ContentType))
            return (false, "Formato inválido. Use JPG, PNG ou WEBP.");

        var tamanhoMaximo = 5 * 1024 * 1024; // 5MB
        if (imagem.Length > tamanhoMaximo)
            return (false, "Imagem não pode ter mais que 5MB.");

        Console.WriteLine($"Imagem válida: {imagem.FileName}, Tipo: {imagem.ContentType}, Tamanho: {imagem.Length} bytes");

        return (true, string.Empty);
    }
}