using PackageFlow.Core.Enums;
using PackageFlow.Core.Helpers;
using PackageFlow.Core.Models;
using PackageFlow.Data.Context;

namespace PackageFlow.Data.Seed;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Users.Any()) return;

        var defaultUsers = new List<AppUser>
        {
            new()
            {
                Username = "admin",
                Email = "admin@packageflow.local",
                FullName = "Adminisztrátor",
                PhoneNumber = "+36301111111",
                PasswordHash = PasswordHelper.Hash("Admin123!"),
                Role = UserRole.Admin
            },
            new()
            {
                Username = "courier1",
                Email = "courier@packageflow.local",
                FullName = "Kovács Béla",
                PhoneNumber = "+36302222222",
                PasswordHash = PasswordHelper.Hash("Courier123!"),
                Role = UserRole.Courier
            },
            new()
            {
                Username = "warehouse1",
                Email = "warehouse@packageflow.local",
                FullName = "Nagy Géza",
                PhoneNumber = "+36303333333",
                PasswordHash = PasswordHelper.Hash("Warehouse123!"),
                Role = UserRole.Warehouseman
            },
            new()
            {
                Username = "customer1",
                Email = "customer@packageflow.local",
                FullName = "Teszt Elek",
                PhoneNumber = "+36304444444",
                PasswordHash = PasswordHelper.Hash("Customer123!"),
                Role = UserRole.Customer
            }
        };

        context.Users.AddRange(defaultUsers);
        context.SaveChanges();

        var courier = defaultUsers.First(u => u.Role == UserRole.Courier);
        var warehouseman = defaultUsers.First(u => u.Role == UserRole.Warehouseman);
        var customer = defaultUsers.First(u => u.Role == UserRole.Customer);

        var defaultWarehouses = new List<Warehouse>
        {
            new()
            {
                Name = "Budapest Központi Depó",
                MaxCapacity = 500,
                Address = new Address
                {
                    Country = "Magyarország",
                    PostalCode = 1107,
                    State = "Pest",
                    City = "Budapest",
                    Street = "Bihari utca",
                    StreetNumber = 8,
                    BuildingDetails = "3-as csarnok"
                }
            }
        };

        context.Warehouses.AddRange(defaultWarehouses);
        context.SaveChanges();

        var centralWarehouse = defaultWarehouses.First();

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);

        var defaultPackages = new List<Package>
        {
            new()
            {
                TrackingNumber = "PKF-2026-0001",
                Status = PackageStatus.Registered,
                Size = PackageSize.Small,
                WeightKg = 1.25m,
                ScheduledPickupDate = today.AddDays(1),
                SenderName = "Kiss Mária (Vendég)",
                SenderEmail = "maria.kiss@example.com",
                SenderPhone = "+36205556677",
                SenderAddress = new Address
                {
                    Country = "Magyarország",
                    PostalCode = 1181,
                    State = "Pest",
                    City = "Budapest",
                    Street = "Üllői út",
                    StreetNumber = 450,
                    BuildingDetails = "1. em. 3."
                },
                RecipientName = "Tóth Péter",
                RecipientEmail = "peter.toth@example.com",
                RecipientPhone = "+36701234567",
                DeliveryAddress = new Address
                {
                    Country = "Magyarország",
                    PostalCode = 6720,
                    State = "Csongrád-Csanád",
                    City = "Szeged",
                    Street = "Kárász utca",
                    StreetNumber = 14
                },
                CreatedAt = now.AddHours(-3),
                StatusHistory = new List<PackageStatusHistory>
                {
                    new()
                    {
                        Status = PackageStatus.Registered,
                        Timestamp = now.AddHours(-3),
                        Note = "Csomag előjegyezve a felületen"
                    }
                }
            },

            new()
            {
                TrackingNumber = "PKF-2026-0002",
                Status = PackageStatus.InWarehouse,
                Size = PackageSize.Large,
                WeightKg = 8.50m,
                ScheduledPickupDate = today,
                SenderUserId = customer.Id,
                SenderName = customer.FullName,
                SenderEmail = customer.Email,
                SenderPhone = customer.PhoneNumber ?? string.Empty,
                SenderAddress = new Address
                {
                    Country = "Magyarország",
                    PostalCode = 1052,
                    State = "Pest",
                    City = "Budapest",
                    Street = "Váci utca",
                    StreetNumber = 10
                },
                RecipientName = "Horváth Anna",
                RecipientEmail = "anna.horvath@example.com",
                RecipientPhone = "+36209876543",
                DeliveryAddress = new Address
                {
                    Country = "Magyarország",
                    PostalCode = 4024,
                    State = "Hajdú-Bihar",
                    City = "Debrecen",
                    Street = "Piac utca",
                    StreetNumber = 22,
                    BuildingDetails = "12-es csengő"
                },
                PickupCourierId = courier.Id,
                WarehouseId = centralWarehouse.Id,
                CreatedAt = now.AddDays(-1),
                PickedUpAt = now.AddHours(-4),
                ReceivedInWarehouseAt = now.AddHours(-2),
                StatusHistory = new List<PackageStatusHistory>
                {
                    new()
                    {
                        Status = PackageStatus.Registered,
                        Timestamp = now.AddDays(-1),
                        ChangedByUserId = customer.Id
                    },
                    new()
                    {
                        Status = PackageStatus.PickupAssigned,
                        Timestamp = now.AddHours(-6),
                        ChangedByUserId = warehouseman.Id,
                        Note = "Felvétel kiosztva Kovács Béla futárnak"
                    },
                    new()
                    {
                        Status = PackageStatus.PickedUp,
                        Timestamp = now.AddHours(-4),
                        ChangedByUserId = courier.Id
                    },
                    new()
                    {
                        Status = PackageStatus.InWarehouse,
                        Timestamp = now.AddHours(-2),
                        ChangedByUserId = warehouseman.Id,
                        Note = "Beérkezett a depóba és betárolva"
                    }
                }
            },

            new()
            {
                TrackingNumber = "PKF-2026-0003",
                Status = PackageStatus.OutForDelivery,
                Size = PackageSize.Medium,
                WeightKg = 3.10m,
                ScheduledPickupDate = today.AddDays(-1),
                SenderName = "Szabó Zoltán",
                SenderEmail = "zoltan.szabo@example.com",
                SenderPhone = "+36307654321",
                SenderAddress = new Address
                {
                    Country = "Magyarország",
                    PostalCode = 1134,
                    State = "Pest",
                    City = "Budapest",
                    Street = "Váci út",
                    StreetNumber = 45
                },
                RecipientUserId = customer.Id,
                RecipientName = customer.FullName,
                RecipientEmail = customer.Email,
                RecipientPhone = customer.PhoneNumber ?? string.Empty,
                DeliveryAddress = new Address
                {
                    Country = "Magyarország",
                    PostalCode = 1182,
                    State = "Pest",
                    City = "Budapest",
                    Street = "Petőfi utca",
                    StreetNumber = 3
                },
                PickupCourierId = courier.Id,
                DeliveryCourierId = courier.Id,
                WarehouseId = centralWarehouse.Id,
                CreatedAt = now.AddDays(-2),
                PickedUpAt = now.AddDays(-1),
                ReceivedInWarehouseAt = now.AddHours(-18),
                OutForDeliveryAt = now.AddHours(-1),
                StatusHistory = new List<PackageStatusHistory>
                {
                    new()
                    {
                        Status = PackageStatus.Registered,
                        Timestamp = now.AddDays(-2)
                    },
                    new()
                    {
                        Status = PackageStatus.InWarehouse,
                        Timestamp = now.AddHours(-18),
                        ChangedByUserId = warehouseman.Id
                    },
                    new()
                    {
                        Status = PackageStatus.DeliveryAssigned,
                        Timestamp = now.AddHours(-3),
                        ChangedByUserId = warehouseman.Id
                    },
                    new()
                    {
                        Status = PackageStatus.OutForDelivery,
                        Timestamp = now.AddHours(-1),
                        ChangedByUserId = courier.Id,
                        Note = "Futár megkezdte a kiszállítást"
                    }
                }
            }
        };

        context.Packages.AddRange(defaultPackages);
        context.SaveChanges();

        var defaultCapacityLogs = new List<WarehouseCapacityLog>
        {
            new()
            {
                WarehouseId = centralWarehouse.Id,
                LoggedAt = now.AddDays(-2),
                CurrentPackageCount = 188,
                MaxCapacitySnapshot = centralWarehouse.MaxCapacity
            },
            new()
            {
                WarehouseId = centralWarehouse.Id,
                LoggedAt = now.AddDays(-1),
                CurrentPackageCount = 215,
                MaxCapacitySnapshot = centralWarehouse.MaxCapacity
            },
            new()
            {
                WarehouseId = centralWarehouse.Id,
                LoggedAt = now,
                CurrentPackageCount = 1,
                MaxCapacitySnapshot = centralWarehouse.MaxCapacity
            }
        };

        context.WarehouseCapacityLogs.AddRange(defaultCapacityLogs);
        context.SaveChanges();
    }
}
