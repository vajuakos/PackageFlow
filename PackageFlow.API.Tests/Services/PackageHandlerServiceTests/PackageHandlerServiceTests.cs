using Microsoft.Extensions.Logging.Abstractions;
using PackageFlow.API.Models;
using PackageFlow.API.Services.PackageHandler;
using PackageFlow.Shared.DTOs.PackageHandler;
using PackageFlow.Shared.Enums;

namespace PackageFlow.API.Tests.Services.PackageHandlerServiceTests
{
    public class PackageHandlerServiceTests : ServiceTestBase
    {
        // Dummy logger instance to fill the missing dependency
        private readonly NullLogger<PackageHandlerService> _logger;
        private readonly PackageHandlerService _packageHandlerService;

        public PackageHandlerServiceTests()
        {
            _logger = NullLogger<PackageHandlerService>.Instance;
            _packageHandlerService = new PackageHandlerService(_context, _logger);
        }

        [Fact]
        public async Task CreatePackageAsync_WithValidRequest_Succeeds()
        {
            // Arrange
            var testUser = new AppUser
            {
                Id = _userId,
                Email = "teszt@teszt.hu",
            };

            _context.Users.Add(testUser);
            await _context.SaveChangesAsync();

            var request = new CreatePackageRequest
            {
                SenderName = "Teszt Feladó",
                SenderEmail = "felado@teszt.hu",
                SenderPhone = "+36301234567",
                SenderCountry = "Magyarország",
                SenderState = "Budapest",
                SenderPostalCode = "1181",
                SenderCity = "Budapest",
                SenderStreet = "Példa utca",
                SenderStreetNumber = "10",

                RecipientName = "Teszt Címzett",
                RecipientEmail = "cimzett@teszt.hu",
                RecipientPhone = "+36209876543",
                DeliveryCountry = "Magyarország",
                DeliveryState = "Veszprém",
                DeliveryPostalCode = "8200",
                DeliveryCity = "Veszprém",
                DeliveryStreet = "Egyetem utca",
                DeliveryStreetNumber = "1",

                SizeCategory = PackageSize.Medium,
                WeightKg = 2.5m,
                ScheduledPickupDate = DateTime.Now.AddDays(2)
            };

            // Act
            var result = await _packageHandlerService.CreatePackageAsync(request, _userId);

            // Assert
            Assert.True(result);
            Assert.Equal(1, _context.Packages.Count());
        }

        [Fact]
        public async Task GetPackageByTrackingNumberAsync_WithValidTrackingNumber_ReturnsExistingPackage()
        {
            // Arrange
            string trackingNumber = "TRACK-001";

            var existingPackage = new Package
            {
                TrackingNumber = trackingNumber,
                WeightKg = 3.5m,
                Status = PackageStatus.Registered
            };

            _context.Packages.Add(existingPackage);
            await _context.SaveChangesAsync();

            // Act
            var returnedPackage = await _packageHandlerService.GetPackageByTrackingNumberAsync(trackingNumber);

            // Assert
            Assert.NotNull(returnedPackage);
            Assert.Equal(trackingNumber, returnedPackage.TrackingNumber);
            Assert.Equal(existingPackage.WeightKg, returnedPackage.WeightKg);
        }

        [Fact]
        public async Task GetPackageByTrackingNumberAsync_WithEmptyTrackingNumber_ThrowsException()
        {
            // Arrange
            string trackingNumber = string.Empty;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _packageHandlerService.GetPackageByTrackingNumberAsync(trackingNumber));

            Assert.Contains("trackingNumber", exception.Message);
        }
    }
}
