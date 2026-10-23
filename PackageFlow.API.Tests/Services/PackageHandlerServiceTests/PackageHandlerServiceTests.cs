using PackageFlow.API.Models;
using PackageFlow.API.Services.PackageHandler;
using PackageFlow.Shared.DTOs.PackageHandler;
using PackageFlow.Shared.Enums;

namespace PackageFlow.API.Tests.Services.PackageHandlerServiceTests
{
    public class PackageHandlerServiceTests : ServiceTestBase
    {
        private readonly PackageHandlerService _service;

        public PackageHandlerServiceTests()
        {
            _service = new PackageHandlerService(_context);
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
            var result = await _service.CreatePackageAsync(request, _userId);

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
            var returnedPackage = await _service.GetPackageByTrackingNumberAsync(trackingNumber);

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
                _service.GetPackageByTrackingNumberAsync(trackingNumber));

            Assert.Contains("trackingNumber", exception.Message);
        }
    }
}
