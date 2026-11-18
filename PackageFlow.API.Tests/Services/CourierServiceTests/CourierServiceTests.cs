using PackageFlow.API.Models;
using PackageFlow.API.Services.Courier;
using PackageFlow.Shared.Enums;

namespace PackageFlow.API.Tests.Services.CourierServiceTests
{
    public class CourierServiceTests : ServiceTestBase
    {
        private readonly CourierService _courierService;

        public CourierServiceTests()
        {
            _courierService = new CourierService(_context);
        }

        private static Address CreateTestAddress() => new()
        {
            Country = "Magyarország",
            State = "Fejér",
            PostalCode = "8000",
            City = "Székesfehérvár",
            Street = "Fő utca",
            StreetNumber = "1"
        };

        private async Task<AppUser> SeedCourierAsync(int id = 10, string email = "futar@teszt.hu")
        {
            var courier = new AppUser
            {
                Id = id,
                FullName = "Futár István",
                Email = email
            };
            _context.Users.Add(courier);
            await _context.SaveChangesAsync();
            return courier;
        }

        [Fact]
        public async Task GetAssignedPackagesAsync_ReturnsOnlyPackagesAssignedToGivenCourier()
        {
            // Arrange
            var courier1 = await SeedCourierAsync(10, "futar1@teszt.hu");
            var courier2 = await SeedCourierAsync(20, "futar2@teszt.hu");

            _context.Packages.AddRange(
                new Package
                {
                    TrackingNumber = "PKF20261118AB0001",
                    Status = PackageStatus.PickupAssigned,
                    PickupCourierId = courier1.Id,
                    WeightKg = 1.5m,
                    SenderName = "Feladó Ferenc",
                    RecipientName = "Címzett Cili",
                    SenderAddress = CreateTestAddress(),
                    DeliveryAddress = CreateTestAddress()
                },
                new Package
                {
                    TrackingNumber = "PKF20261117AB0002",
                    Status = PackageStatus.DeliveryAssigned,
                    DeliveryCourierId = courier1.Id,
                    WeightKg = 2.0m,
                    SenderName = "Feladó Flórián",
                    RecipientName = "Címzett Csaba",
                    SenderAddress = CreateTestAddress(),
                    DeliveryAddress = CreateTestAddress()
                },
                new Package
                {
                    TrackingNumber = "PKF20261116AB0003",
                    Status = PackageStatus.PickupAssigned,
                    PickupCourierId = courier2.Id,
                    WeightKg = 3.0m,
                    SenderName = "Feladó Fülöp",
                    RecipientName = "Címzett Cézár",
                    SenderAddress = CreateTestAddress(),
                    DeliveryAddress = CreateTestAddress()
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _courierService.GetAssignedPackagesAsync(courier1.Id);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.DoesNotContain(result, p => p.TrackingNumber == "PKF20261116AB0003");
        }

        [Fact]
        public async Task GetAssignedPackagesAsync_WithInvalidCourierId_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                _courierService.GetAssignedPackagesAsync(0));
        }

        [Fact]
        public async Task RecordPickupAsync_WithValidPickupAssignedPackage_UpdatesStatusAndCreatesHistory()
        {
            // Arrange
            var courier = await SeedCourierAsync();
            var package = new Package
            {
                TrackingNumber = "PKF20261115AB0010",
                Status = PackageStatus.PickupAssigned,
                PickupCourierId = courier.Id,
                WeightKg = 2.0m,
                SenderName = "Feladó Félix",
                RecipientName = "Címzett Ciceró",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courierService.RecordPickupAsync("PKF20261115AB0010", courier.Id);

            // Assert
            Assert.True(result);
            var updated = await _context.Packages.FindAsync(package.Id);
            Assert.Equal(PackageStatus.PickedUp, updated!.Status);
            Assert.NotNull(updated.PickedUpAt);
            Assert.Single(_context.PackageStatusHistories, h => h.PackageId == package.Id && h.Status == PackageStatus.PickedUp);
        }

        [Fact]
        public async Task RecordOutForDeliveryAsync_WithValidDeliveryAssignedPackage_UpdatesStatus()
        {
            // Arrange
            var courier = await SeedCourierAsync();
            var package = new Package
            {
                TrackingNumber = "PKF20261114AB0020",
                Status = PackageStatus.DeliveryAssigned,
                DeliveryCourierId = courier.Id,
                WeightKg = 2.0m,
                SenderName = "Feladó Fióna",
                RecipientName = "Címzett Cintia",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courierService.RecordOutForDeliveryAsync("PKF20261114AB0020", courier.Id);

            // Assert
            Assert.True(result);
            var updated = await _context.Packages.FindAsync(package.Id);
            Assert.Equal(PackageStatus.OutForDelivery, updated!.Status);
            Assert.NotNull(updated.OutForDeliveryAt);
        }

        [Fact]
        public async Task RecordDeliveryAsync_WithOutForDeliveryPackage_MarksAsDelivered()
        {
            // Arrange
            var courier = await SeedCourierAsync();
            var package = new Package
            {
                TrackingNumber = "PKF20261113AB0030",
                Status = PackageStatus.OutForDelivery,
                DeliveryCourierId = courier.Id,
                WeightKg = 1.2m,
                SenderName = "Feladó Flóra",
                RecipientName = "Címzett Cinka",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courierService.RecordDeliveryAsync("PKF20261113AB0030", courier.Id);

            // Assert
            Assert.True(result);
            var updated = await _context.Packages.FindAsync(package.Id);
            Assert.Equal(PackageStatus.Delivered, updated!.Status);
            Assert.NotNull(updated.DeliveredAt);
        }

        [Fact]
        public async Task RecordFailedDeliveryAsync_WithCustomNote_SetsFailedStatusAndSavesNote()
        {
            // Arrange
            var courier = await SeedCourierAsync();
            var package = new Package
            {
                TrackingNumber = "PKF20261112AB0040",
                Status = PackageStatus.OutForDelivery,
                DeliveryCourierId = courier.Id,
                WeightKg = 4.0m,
                SenderName = "Feladó Fruzsina",
                RecipientName = "Címzett Celesztina",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courierService.RecordFailedDeliveryAsync("PKF20261112AB0040", courier.Id, "Címzett nem volt otthon");

            // Assert
            Assert.True(result);
            var updated = await _context.Packages.FindAsync(package.Id);
            Assert.Equal(PackageStatus.Failed, updated!.Status);

            var history = _context.PackageStatusHistories.Single(h => h.PackageId == package.Id);
            Assert.Equal(PackageStatus.Failed, history.Status);
            Assert.Equal("Címzett nem volt otthon", history.Note);
        }

        [Fact]
        public async Task RecordDeliveryAsync_WhenWrongStatus_ReturnsFalse()
        {
            // Arrange
            var courier = await SeedCourierAsync();
            var package = new Package
            {
                TrackingNumber = "PKF20261110AB0050",
                Status = PackageStatus.InWarehouse,
                DeliveryCourierId = courier.Id,
                WeightKg = 1.0m,
                SenderName = "Feladó Farkas",
                RecipientName = "Címzett Cukorka",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courierService.RecordDeliveryAsync("PKF20261110AB0050", courier.Id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetShippingHistoryAsync_WithValidDateRange_ReturnsMatchingRecords()
        {
            // Arrange
            var courier = await SeedCourierAsync();
            var package = new Package
            {
                TrackingNumber = "PKF20261109AB0060",
                Status = PackageStatus.Delivered,
                WeightKg = 1.0m,
                SenderName = "Feladó Fábián",
                RecipientName = "Címzett Cipra",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            var now = DateTime.UtcNow;
            _context.PackageStatusHistories.Add(new PackageStatusHistory
            {
                PackageId = package.Id,
                Status = PackageStatus.Delivered,
                Timestamp = now,
                ChangedByUserId = courier.Id,
                Note = "Kézbesítve"
            });
            await _context.SaveChangesAsync();

            // Act
            var history = await _courierService.GetShippingHistoryAsync(courier.Id, now.AddHours(-1), now.AddHours(1));

            // Assert
            Assert.Single(history);
            Assert.Equal("PKF20261109AB0060", history[0].TrackingNumber);
        }
    }
}