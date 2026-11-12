using Moq;
using PackageFlow.API.Services.PackageHandler;
using PackageFlow.API.Services.PackageLabelGenerator;
using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.DTOs.PackageHandler;
using QuestPDF.Infrastructure;
using System.Text;

namespace PackageFlow.API.Tests.Services.PackageLabelGeneratorServiceTests
{
    public class PackageLabelGeneratorServiceTests
    {
        private readonly Mock<IPackageHandlerService> _packageHandlerMock;
        private readonly PackageLabelGeneratorService _labelGeneratorService;

        public PackageLabelGeneratorServiceTests()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            _packageHandlerMock = new Mock<IPackageHandlerService>();
            _labelGeneratorService = new PackageLabelGeneratorService(_packageHandlerMock.Object);
        }

        [Fact]
        public void Constructor_WithNullPackageHandlerService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PackageLabelGeneratorService(null!));
        }

        [Fact]
        public async Task GenerateLabelAsync_WithValidTrackingNumber_ReturnsValidPdfBytes()
        {
            string trackingNumber = "TRACK-12345";

            var mockPackage = new PackageResult
            {
                TrackingNumber = trackingNumber,
                SenderName = "Teszt Feladó",
                SenderPhone = "+36301234567",
                SenderAddress = new AddressResult
                {
                    Street = "Példa utca",
                    StreetNumber = "10",
                    BuildingDetails = "2. em. 4.",
                    PostalCode = "1181",
                    City = "Budapest"
                },
                RecipientName = "Teszt Címzett",
                RecipientPhone = "+36209876543",
                DeliveryAddress = new AddressResult
                {
                    Street = "Egyetem utca",
                    StreetNumber = "1",
                    BuildingDetails = "",
                    PostalCode = "8200",
                    City = "Veszprém"
                }
            };

            _packageHandlerMock
                .Setup(s => s.GetPackageByTrackingNumberAsync(trackingNumber))
                .ReturnsAsync(mockPackage);

            byte[] pdfBytes = await _labelGeneratorService.GenerateLabelAsync(trackingNumber);

            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);

            string pdfHeader = Encoding.ASCII.GetString(pdfBytes.Take(4).ToArray());
            Assert.Equal("%PDF", pdfHeader);

            _packageHandlerMock.Verify(s => s.GetPackageByTrackingNumberAsync(trackingNumber), Times.Once);
        }

        [Fact]
        public async Task GenerateLabelAsync_WhenPackageDoesNotExist_ThrowsKeyNotFoundException()
        {
            string trackingNumber = "NON-EXISTENT";

            _packageHandlerMock
                .Setup(s => s.GetPackageByTrackingNumberAsync(trackingNumber))
                .ReturnsAsync((PackageResult)null!);

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _labelGeneratorService.GenerateLabelAsync(trackingNumber));

            Assert.Contains(trackingNumber, exception.Message);
        }

        [Fact]
        public async Task GenerateLabelAsync_WhenPackageHasEmptyTrackingNumber_ThrowsInvalidOperationException()
        {
            string trackingNumber = "TRACK-BROKEN";

            var brokenPackage = new PackageResult
            {
                TrackingNumber = string.Empty,
                SenderAddress = new AddressResult(),
                DeliveryAddress = new AddressResult()
            };

            _packageHandlerMock
                .Setup(s => s.GetPackageByTrackingNumberAsync(trackingNumber))
                .ReturnsAsync(brokenPackage);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _labelGeneratorService.GenerateLabelAsync(trackingNumber));
        }
    }
}
