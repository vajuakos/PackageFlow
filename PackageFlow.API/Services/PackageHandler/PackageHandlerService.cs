using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Data;
using PackageFlow.API.Helpers;
using PackageFlow.API.Models;
using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.DTOs.PackageHandler;
using PackageFlow.Shared.Enums;

namespace PackageFlow.API.Services.PackageHandler
{
    public class PackageHandlerService : IPackageHandlerService
    {
        private readonly AppDbContext _dbContext;

        public PackageHandlerService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<bool> CreatePackageAsync(CreatePackageRequest request, int? userId)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // Find recipient if exists in the database for user profile association
            var recipientId = await _dbContext.Users
                .Where(u => u.Email == request.RecipientEmail.ToLower())
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync();

            var trackingNumber = TrackingNumberHelper.Generate();

            var package = new Package
            {
                TrackingNumber = trackingNumber,
                Status = PackageStatus.Registered,
                Size = request.SizeCategory,
                WeightKg = request.WeightKg,
                ScheduledPickupDate = DateOnly.FromDateTime(request.ScheduledPickupDate),
                CreatedAt = DateTime.UtcNow,

                SenderUserId = userId,

                SenderName = request.SenderName,
                SenderEmail = request.SenderEmail,
                SenderPhone = request.SenderPhone,
                SenderAddress = new Address
                {
                    Country = request.SenderCountry,
                    State = request.SenderState,
                    PostalCode = request.SenderPostalCode,
                    City = request.SenderCity,
                    Street = request.SenderStreet,
                    StreetNumber = request.SenderStreetNumber,
                    BuildingDetails = request.SenderBuildingDetails
                },

                // Assign registered user if exists
                RecipientUserId = recipientId,
                RecipientName = request.RecipientName,
                RecipientEmail = request.RecipientEmail,
                RecipientPhone = request.RecipientPhone,
                DeliveryAddress = new Address
                {
                    Country = request.DeliveryCountry,
                    State = request.DeliveryState,
                    PostalCode = request.DeliveryPostalCode,
                    City = request.DeliveryCity,
                    Street = request.DeliveryStreet,
                    StreetNumber = request.DeliveryStreetNumber,
                    BuildingDetails = request.DeliveryBuildingDetails
                }
            };

            package.StatusHistory.Add(new PackageStatusHistory
            {
                Status = PackageStatus.Registered,
                Timestamp = DateTime.UtcNow,
            });

            await _dbContext.Packages.AddAsync(package);
            var savedChanges = await _dbContext.SaveChangesAsync();

            return savedChanges > 0;
        }

        public async Task<PackageResult?> GetPackageByTrackingNumberAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentNullException(nameof(trackingNumber), "Tracking number must be provided!");

            var cleanTrackingNumber = trackingNumber.Trim();

            return await _dbContext.Packages
                .AsNoTracking()
                .Where(p => p.TrackingNumber == cleanTrackingNumber)
                .Select(p => new PackageResult
                {
                    Status = p.Status,
                    Size = p.Size,
                    WeightKg = p.WeightKg,
                    ScheduledPickupDate = p.ScheduledPickupDate,

                    SenderUser = p.SenderUser != null ? new UserResult
                    {
                        FullName = p.SenderUser.FullName,
                        Email = p.SenderUser.Email
                    } : null,
                    SenderName = p.SenderName,
                    SenderEmail = p.SenderEmail,
                    SenderPhone = p.SenderPhone,
                    SenderAddress = new AddressResult
                    {
                        Country = p.SenderAddress.Country,
                        State = p.SenderAddress.State,
                        PostalCode = p.SenderAddress.PostalCode,
                        City = p.SenderAddress.City,
                        Street = p.SenderAddress.Street,
                        StreetNumber = p.SenderAddress.StreetNumber,
                        BuildingDetails = p.SenderAddress.BuildingDetails
                    },

                    RecipientUser = p.RecipientUser != null ? new UserResult
                    {
                        FullName = p.RecipientUser.FullName,
                        Email = p.RecipientUser.Email
                    } : null,
                    RecipientName = p.RecipientName,
                    RecipientEmail = p.RecipientEmail,
                    RecipientPhone = p.RecipientPhone,
                    DeliveryAddress = new AddressResult
                    {
                        Country = p.DeliveryAddress.Country,
                        State = p.DeliveryAddress.State,
                        PostalCode = p.DeliveryAddress.PostalCode,
                        City = p.DeliveryAddress.City,
                        Street = p.DeliveryAddress.Street,
                        StreetNumber = p.DeliveryAddress.StreetNumber,
                        BuildingDetails = p.DeliveryAddress.BuildingDetails
                    },

                    PickupCourier = p.PickupCourier != null ? new UserResult
                    {
                        FullName = p.PickupCourier.FullName,
                        Email = p.PickupCourier.Email
                    } : null,

                    DeliveryCourier = p.DeliveryCourier != null ? new UserResult
                    {
                        FullName = p.DeliveryCourier.FullName,
                        Email = p.DeliveryCourier.Email
                    } : null,

                    Warehouse = p.Warehouse != null ? new WarehouseResult
                    {
                        Name = p.Warehouse.Name
                    } : null,

                    CreatedAt = p.CreatedAt,
                    PickedUpAt = p.PickedUpAt,
                    ReceivedInWarehouseAt = p.ReceivedInWarehouseAt,
                    OutForDeliveryAt = p.OutForDeliveryAt,
                    DeliveredAt = p.DeliveredAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<PackageResult>> GetPackagesForUserAsync(int? userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId), "Invalid given user ID!");

            return await _dbContext.Packages
                .AsNoTracking()
                .Where(p => p.SenderUserId == userId ||
                    p.RecipientUserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PackageResult
                {
                    TrackingNumber = p.TrackingNumber,

                    Status = p.Status,
                    Size = p.Size,
                    WeightKg = p.WeightKg,
                    ScheduledPickupDate = p.ScheduledPickupDate,

                    SenderUser = p.SenderUser != null ? new UserResult
                    {
                        FullName = p.SenderUser.FullName,
                        Email = p.SenderUser.Email
                    } : null,
                    SenderName = p.SenderName,
                    SenderEmail = p.SenderEmail,
                    SenderPhone = p.SenderPhone,
                    SenderAddress = new AddressResult
                    {
                        Country = p.SenderAddress.Country,
                        State = p.SenderAddress.State,
                        PostalCode = p.SenderAddress.PostalCode,
                        City = p.SenderAddress.City,
                        Street = p.SenderAddress.Street,
                        StreetNumber = p.SenderAddress.StreetNumber,
                        BuildingDetails = p.SenderAddress.BuildingDetails
                    },

                    RecipientUser = p.RecipientUser != null ? new UserResult
                    {
                        FullName = p.RecipientUser.FullName,
                        Email = p.RecipientUser.Email
                    } : null,
                    RecipientName = p.RecipientName,
                    RecipientEmail = p.RecipientEmail,
                    RecipientPhone = p.RecipientPhone,
                    DeliveryAddress = new AddressResult
                    {
                        Country = p.DeliveryAddress.Country,
                        State = p.DeliveryAddress.State,
                        PostalCode = p.DeliveryAddress.PostalCode,
                        City = p.DeliveryAddress.City,
                        Street = p.DeliveryAddress.Street,
                        StreetNumber = p.DeliveryAddress.StreetNumber,
                        BuildingDetails = p.DeliveryAddress.BuildingDetails
                    },

                    PickupCourier = p.PickupCourier != null ? new UserResult
                    {
                        FullName = p.PickupCourier.FullName,
                        Email = p.PickupCourier.Email
                    } : null,

                    DeliveryCourier = p.DeliveryCourier != null ? new UserResult
                    {
                        FullName = p.DeliveryCourier.FullName,
                        Email = p.DeliveryCourier.Email
                    } : null,

                    Warehouse = p.Warehouse != null ? new WarehouseResult
                    {
                        Name = p.Warehouse.Name
                    } : null,

                    CreatedAt = p.CreatedAt,
                    PickedUpAt = p.PickedUpAt,
                    ReceivedInWarehouseAt = p.ReceivedInWarehouseAt,
                    OutForDeliveryAt = p.OutForDeliveryAt,
                    DeliveredAt = p.DeliveredAt
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<PackageStatusHistoryResult>> GetPackageStatusHistoryAsync(string trackingNumber)
        {
            return await _dbContext.PackageStatusHistories
                .AsNoTracking()
                .Where(h => h.Package.TrackingNumber == trackingNumber)
                .OrderByDescending(h => h.Timestamp)
                .Select(h => new PackageStatusHistoryResult
                {
                    PackageId = h.PackageId,
                    Status = h.Status,
                    Timestamp = h.Timestamp,
                    Note = h.Note,
                    ChangedByUser = h.ChangedByUser != null ? new UserResult
                    {
                        FullName = h.ChangedByUser.FullName,
                        Email = h.ChangedByUser.Email
                    } : null
                })
                .ToListAsync();
        }

        public async Task UpdatePackageDetailsAsync(string trackingNumber, UpdatePackageRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var existingPackage = await _dbContext.Packages
                .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);

            if (existingPackage is null)
                throw new KeyNotFoundException("Package not found.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (existingPackage.ScheduledPickupDate <= today)
                throw new InvalidOperationException("The modification deadline has expired.");

            var newPickupDate = DateOnly.FromDateTime(request.ScheduledPickupDate);

            existingPackage.ScheduledPickupDate = newPickupDate;
            existingPackage.Size = request.Size;

            _dbContext.PackageStatusHistories.Add(new PackageStatusHistory
            {
                PackageId = existingPackage.Id,
                Status = PackageStatus.Modified,
                Timestamp = DateTime.UtcNow,
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
