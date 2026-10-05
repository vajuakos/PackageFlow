using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Models;
using PackageFlow.Shared.Constants;
using PackageFlow.Shared.Enums;

namespace PackageFlow.API.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roles = [
                UserRoles.Customer,
                UserRoles.Courier,
                UserRoles.WarehouseManager,
                UserRoles.Admin
            ];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            // Early return if any users exists
            if (await userManager.Users.AnyAsync()) return;

            var admin = await CreateUserWithRole(userManager, "admin@packageflow.local", "Adminisztrátor", "Admin123!", UserRoles.Admin);
            var courierUser = await CreateUserWithRole(userManager, "courier@packageflow.local", "Kovács Béla", "Courier123!", UserRoles.Courier);
            var warehouseman = await CreateUserWithRole(userManager, "warehouse@packageflow.local", "Nagy Géza", "Warehouse123!", UserRoles.WarehouseManager);
            var customer = await CreateUserWithRole(userManager, "customer@packageflow.local", "Teszt Elek", "Customer123!", UserRoles.Customer);

            if (!await context.CourierProfiles.AnyAsync(c => c.UserId == courierUser.Id))
            {
                context.CourierProfiles.Add(new CourierProfile
                {
                    UserId = courierUser.Id,
                    VehiclePlateNumber = "AA-PF-123"
                });
                await context.SaveChangesAsync();
            }

            if (!await context.Warehouses.AnyAsync())
            {
                var centralWarehouse = new Warehouse
                {
                    Name = "Budapest Központi Depó",
                    MaxCapacity = 500,
                    Address = new Address
                    {
                        Country = "Magyarország",
                        PostalCode = "1107",
                        State = "Pest",
                        City = "Budapest",
                        Street = "Bihari utca",
                        StreetNumber = "8",
                        BuildingDetails = "3-as csarnok"
                    }
                };

                context.Warehouses.Add(centralWarehouse);
                await context.SaveChangesAsync();

                await SeedPackagesAsync(context, centralWarehouse.Id, customer.Id, courierUser.Id, warehouseman.Id);
            }
        }

        private static async Task<AppUser> CreateUserWithRole(
            UserManager<AppUser> userManager,
            string email,
            string fullName,
            string password,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"User can not be created! ({email})");
                }

                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }

        private static async Task SeedPackagesAsync(
            AppDbContext context,
            int warehouseId,
            int customerId,
            int courierId,
            int warehousemanId)
        {
            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);

            var packages = new List<Package>
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
                        PostalCode = "1181",
                        City = "Budapest",
                        Street = "Üllői út",
                        StreetNumber = "450"
                    },
                    RecipientName = "Tóth Péter",
                    RecipientEmail = "peter.toth@example.com",
                    RecipientPhone = "+36701234567",
                    DeliveryAddress = new Address
                    {
                        Country = "Magyarország",
                        PostalCode = "6720",
                        City = "Szeged",
                        Street = "Kárász utca",
                        StreetNumber = "14"
                    },
                    CreatedAt = now.AddHours(-3)
                },
                new()
                {
                    TrackingNumber = "PKF-2026-0002",
                    Status = PackageStatus.InWarehouse,
                    Size = PackageSize.Large,
                    WeightKg = 8.50m,
                    ScheduledPickupDate = today,
                    SenderUserId = customerId,
                    SenderName = "Teszt Elek",
                    SenderEmail = "customer@packageflow.local",
                    SenderPhone = "+36304444444",
                    SenderAddress = new Address
                    {
                        Country = "Magyarország",
                        PostalCode = "1052",
                        City = "Budapest",
                        Street = "Váci utca",
                        StreetNumber = "10"
                    },
                    RecipientName = "Horváth Anna",
                    RecipientEmail = "anna.horvath@example.com",
                    RecipientPhone = "+36209876543",
                    DeliveryAddress = new Address
                    {
                        Country = "Magyarország",
                        PostalCode = "4024",
                        City = "Debrecen",
                        Street = "Piac utca",
                        StreetNumber = "22"
                    },
                    PickupCourierId = courierId,
                    WarehouseId = warehouseId,
                    CreatedAt = now.AddDays(-1),
                    PickedUpAt = now.AddHours(-4),
                    ReceivedInWarehouseAt = now.AddHours(-2)
                }
            };

            context.Packages.AddRange(packages);
            await context.SaveChangesAsync();
        }
    }
}
