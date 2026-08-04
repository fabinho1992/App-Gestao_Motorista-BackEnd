using RotaCerta.Domain.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;// já deve estar importado, garante que tem WebpFileFormatType

namespace RotaCerta.Infraestructure.Repository.Storage
{
    public class ImageSharpProcessorService : IImagemProcessorService
    {
        private const int LarguraMaxima = 1280;
        private const int Qualidade = 75;

        public async Task<Stream> ProcessarAsync(Stream imagemOriginal, CancellationToken cancellationToken = default)
        {
            using var image = await Image.LoadAsync(imagemOriginal, cancellationToken);

            image.Mutate(x => x
                .AutoOrient() // corrige rotação usando EXIF antes de remover os metadados
                .Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(LarguraMaxima, LarguraMaxima)
                }));

            // remove metadados (EXIF/GPS) depois de já ter corrigido a orientação
            image.Metadata.ExifProfile = null;
            image.Metadata.IccProfile = null;

            var encoder = new WebpEncoder
            {
                FileFormat = WebpFileFormatType.Lossy,
                Quality = Qualidade
            };

            var outputStream = new MemoryStream();
            await image.SaveAsWebpAsync(outputStream, encoder, cancellationToken);
            outputStream.Position = 0;

            return outputStream;
        }
    }
    }
