using PackageFlow.API.Models;
using PackageFlow.API.Services.WarehouseManager;
using PackageFlow.Shared.Enums;

namespace PackageFlow.API.Tests.Services.WarehouseManagerServiceTests
{
    public class WarehouseManagerServiceTests : ServiceTestBase
    {
        private readonly WarehouseManagerService _warehouseService;

        public WarehouseManagerServiceTests()
        {
            _warehouseService = new WarehouseManagerService(_context);
        }

        private static Address CreateTestAddress() => new()
        {
            Country = "Magyarország",
            State = "Veszprém",
            PostalCode = "8200",
            City = "Veszprém",
            Street = "Raktár köz",
            StreetNumber = "5"
        };

        private async Task<(AppUser manager, Warehouse warehouse)> SeedWarehouseAndManagerAsync()
        {
            var manager = new AppUser
            {
                Id = _userId,
                FullName = "Raktáros Robi",
                Email = "raktaros@teszt.hu"
            };
            var warehouse = new Warehouse
            {
                Name = "Központi Raktár",
                Address = CreateTestAddress()
            };

            _context.Users.Add(manager);
            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            return (manager, warehouse);
        }

        private async Task<AppUser> SeedCourierWithProfileAsync(int courierId = 15, string plate = "ABC-123")
        {
            var courierUser = new AppUser
            {
                Id = courierId,
                FullName = $"Futár {courierId}",
                Email = $"futar{courierId}@teszt.hu"
            };
            var profile = new CourierProfile
            {
                UserId = courierId,
                VehiclePlateNumber = plate
            };

            _context.Users.Add(courierUser);
            _context.CourierProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return courierUser;
        }

        [Fact]
        public async Task RecordWarehouseArrivalAsync_WithPickedUpPackage_SetsInWarehouseAndAssignsWarehouse()
        {
            // Arrange
            var (manager, warehouse) = await SeedWarehouseAndManagerAsync();
            var package = new Package
            {
                TrackingNumber = "PKF-2026-1001",
                Status = PackageStatus.PickedUp,
                WeightKg = 3.0m,
                SenderName = "Feladó",
                RecipientName = "Címzett",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // Act
            var result = await _warehouseService.RecordWarehouseArrivalAsync("PKF-2026-1001", manager.Id);

            // Assert
            Assert.True(result);
            var updated = await _context.Packages.FindAsync(package.Id);
            Assert.Equal(PackageStatus.InWarehouse, updated!.Status);
            Assert.Equal(warehouse.Id, updated.WarehouseId);
            Assert.NotNull(updated.ReceivedInWarehouseAt);
            Assert.Single(_context.PackageStatusHistories, h => h.PackageId == package.Id && h.Status == PackageStatus.InWarehouse);
        }

        [Fact]
        public async Task RecordWarehouseReturnAsync_WithUnderThreeFailedAttempts_RequeuesToInWarehouse()
        {
            // Arrange
            var (manager, warehouse) = await SeedWarehouseAndManagerAsync();
            var courier = await SeedCourierWithProfileAsync();

            var package = new Package
            {
                TrackingNumber = "PKF20261108AB1002",
                Status = PackageStatus.Failed,
                DeliveryCourierId = courier.Id,
                WeightKg = 2.5m,
                SenderName = "Feladó Ferenc",
                RecipientName = "Címzett Csenge",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // 2 korábbi sikertelen kézbesítési kísérlet
            _context.PackageStatusHistories.AddRange(
                new PackageStatusHistory { PackageId = package.Id, Status = PackageStatus.Failed, Timestamp = DateTime.UtcNow.AddDays(-1), ChangedByUserId = courier.Id },
                new PackageStatusHistory { PackageId = package.Id, Status = PackageStatus.Failed, Timestamp = DateTime.UtcNow, ChangedByUserId = courier.Id }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _warehouseService.RecordWarehouseReturnAsync("PKF20261108AB1002", manager.Id);

            // Assert
            Assert.True(result);
            var updated = await _context.Packages.FindAsync(package.Id);
            Assert.Equal(PackageStatus.InWarehouse, updated!.Status);
            Assert.Null(updated.DeliveryCourierId);
            Assert.Equal(warehouse.Id, updated.WarehouseId);
        }

        [Fact]
        public async Task RecordWarehouseReturnAsync_WithThreeFailedAttempts_CancelsPackage()
        {
            // Arrange
            var (manager, _) = await SeedWarehouseAndManagerAsync();
            var courier = await SeedCourierWithProfileAsync();

            var package = new Package
            {
                TrackingNumber = "PKF20261106AB1003",
                Status = PackageStatus.Failed,
                DeliveryCourierId = courier.Id,
                WeightKg = 2.5m,
                SenderName = "Feladó Fruzsi",
                RecipientName = "Címzett Csongor",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // 3 sikertelen kézbesítési kísérlet
            _context.PackageStatusHistories.AddRange(
                new PackageStatusHistory { PackageId = package.Id, Status = PackageStatus.Failed, Timestamp = DateTime.UtcNow.AddDays(-2), ChangedByUserId = courier.Id },
                new PackageStatusHistory { PackageId = package.Id, Status = PackageStatus.Failed, Timestamp = DateTime.UtcNow.AddDays(-1), ChangedByUserId = courier.Id },
                new PackageStatusHistory { PackageId = package.Id, Status = PackageStatus.Failed, Timestamp = DateTime.UtcNow, ChangedByUserId = courier.Id }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _warehouseService.RecordWarehouseReturnAsync("PKF20261106AB1003", manager.Id);

            // Assert
            Assert.True(result);
            var updated = await _context.Packages.FindAsync(package.Id);
            Assert.Equal(PackageStatus.Cancelled, updated!.Status);
            Assert.Null(updated.DeliveryCourierId);
            Assert.Single(_context.PackageStatusHistories, h => h.PackageId == package.Id && h.Status == PackageStatus.Cancelled);
        }

        [Fact]
        public async Task AssignPickupCourierAsync_WithRegisteredPackage_AssignsCourierAndUpdatesStatus()
        {
            // Arrange
            var (manager, _) = await SeedWarehouseAndManagerAsync();
            var courier = await SeedCourierWithProfileAsync();

            var package = new Package
            {
                TrackingNumber = "PKF20261118AB1004",
                Status = PackageStatus.Registered,
                WeightKg = 1.0m,
                SenderName = "Feladó Frida",
                RecipientName = "Címzett Csilla",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // Act
            var result = await _warehouseService.AssignPickupCourierAsync("PKF20261118AB1004", courier.Id, manager.Id);

            // Assert
            Assert.True(result);
            var updated = await _context.Packages.FindAsync(package.Id);
            Assert.Equal(PackageStatus.PickupAssigned, updated!.Status);
            Assert.Equal(courier.Id, updated.PickupCourierId);
        }

        [Fact]
        public async Task AssignDeliveryCourierAsync_WithInWarehousePackage_AssignsCourierAndUpdatesStatus()
        {
            // Arrange
            var (manager, _) = await SeedWarehouseAndManagerAsync();
            var courier = await SeedCourierWithProfileAsync();

            var package = new Package
            {
                TrackingNumber = "PKF20261117AB1005",
                Status = PackageStatus.InWarehouse,
                WeightKg = 4.5m,
                SenderName = "Feladó Feri",
                RecipientName = "Címzett Csanád",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // Act
            var result = await _warehouseService.AssignDeliveryCourierAsync("PKF20261117AB1005", courier.Id, manager.Id);

            // Assert
            Assert.True(result);
            var updated = await _context.Packages.FindAsync(package.Id);
            Assert.Equal(PackageStatus.DeliveryAssigned, updated!.Status);
            Assert.Equal(courier.Id, updated.DeliveryCourierId);
        }

        [Fact]
        public async Task AssignDeliveryCourierAsync_WithNonExistentCourier_ReturnsFalse()
        {
            // Arrange
            var (manager, _) = await SeedWarehouseAndManagerAsync();
            var package = new Package
            {
                TrackingNumber = "PKF20261112AB1006",
                Status = PackageStatus.InWarehouse,
                WeightKg = 1.5m,
                SenderName = "Feladó Farkaska",
                RecipientName = "Címzett Csengele",
                SenderAddress = CreateTestAddress(),
                DeliveryAddress = CreateTestAddress()
            };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // Act
            var result = await _warehouseService.AssignDeliveryCourierAsync("PKF20261112AB1006", 999, manager.Id);

            // Assert
            Assert.False(result);
        }
    }
}