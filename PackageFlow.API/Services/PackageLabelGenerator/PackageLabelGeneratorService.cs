using PackageFlow.API.Services.PackageHandler;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp.Rendering;

namespace PackageFlow.API.Services.PackageLabelGenerator
{
    public class PackageLabelGeneratorService : IPackageLabelGeneratorService
    {
        private readonly IPackageHandlerService _packageHandlerService;

        public PackageLabelGeneratorService(IPackageHandlerService packageHandlerService)
        {
            _packageHandlerService = packageHandlerService ?? throw new ArgumentNullException(nameof(packageHandlerService));
        }

        public async Task<byte[]> GenerateLabelAsync(string trackingNumber)
        {
            var package = await _packageHandlerService.GetPackageByTrackingNumberAsync(trackingNumber);

            if (package == null)
            {
                throw new KeyNotFoundException($"Package with tracking number '{trackingNumber}' was not found.");
            }

            byte[] qrCodeBytes = GenerateQrCodeImage(package.TrackingNumber);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text("PackageFlow Logisztika").FontSize(16).Bold();

                    page.Content().Column(col =>
                    {
                        col.Spacing(15);

                        col.Item().Text($"Csomagszám: {package.TrackingNumber}").FontSize(14).Bold();

                        col.Item().Row(row =>
                        {
                            row.Spacing(20);

                            row.RelativeItem().Column(senderCol =>
                            {
                                senderCol.Spacing(3);
                                senderCol.Item().Text("Feladó:").Bold();
                                senderCol.Item().Text(package.SenderName);
                                senderCol.Item().Text(package.SenderPhone);
                                senderCol.Item().Text($"{package.SenderAddress.Street}, {package.SenderAddress.StreetNumber}, {package.SenderAddress.BuildingDetails}");
                                senderCol.Item().Text($"{package.SenderAddress.PostalCode}, {package.SenderAddress.City}");
                            });

                            row.RelativeItem().Column(recipientCol =>
                            {
                                recipientCol.Spacing(3);
                                recipientCol.Item().Text("Címzett:").Bold();
                                recipientCol.Item().Text(package.RecipientName);
                                recipientCol.Item().Text(package.RecipientPhone);
                                recipientCol.Item().Text($"{package.DeliveryAddress.Street}, {package.DeliveryAddress.StreetNumber}, {package.DeliveryAddress.BuildingDetails}");
                                recipientCol.Item().Text($"{package.DeliveryAddress.PostalCode}, {package.DeliveryAddress.City}");
                            });
                        });

                        col.Item().AlignCenter().PaddingTop(25).Width(110).Height(110).Image(qrCodeBytes);
                    });
                });
            });

            return document.GeneratePdf();
        }

        private byte[] GenerateQrCodeImage(string data)
        {
            try
            {
                var writer = new BarcodeWriter<SKBitmap>
                {
                    Format = BarcodeFormat.QR_CODE,
                    Renderer = new SKBitmapRenderer(),
                    Options = new EncodingOptions
                    {
                        Width = 300,
                        Height = 300,
                        Margin = 1
                    }
                };

                using SKBitmap bitmap = writer.Write(data);

                if (bitmap == null)
                {
                    throw new InvalidOperationException("Failed to generate QR code bitmap: The writer returned a null instance.");
                }

                using var image = SKImage.FromBitmap(bitmap);
                using var encodedData = image.Encode(SKEncodedImageFormat.Png, 100);

                if (encodedData == null)
                {
                    throw new InvalidOperationException("Failed to encode QR code image to PNG format.");
                }

                return encodedData.ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"QR code rendering exception occurred for data '{data}': {ex.Message}", ex);
            }
        }
    }
}
