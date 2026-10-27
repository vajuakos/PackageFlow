using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Data;
using PackageFlow.API.Models;
using PackageFlow.Shared.DTOs.PackageHandler;


namespace PackageFlow.API.Services.Courier
{
    public class CourierService : ICourierService
    {
        private readonly AppDbContext _dbContext;

        public CourierService(AppDbContext dbContext)
        {
            _dbContext = dbContext
                ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<PackageResult>> GetAssignedPackagesAsync(int courierId)
        {
            if (courierId <= 0)
                throw new ArgumentOutOfRangeException(nameof(courierId), "Invalid given courier ID!");

            return await _dbContext.Packages
                .AsNoTracking()
                .Where(p => p.PickupCourierId == courierId
                    || p.DeliveryCourierId == courierId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PackageResult
                {
                    TrackingNumber = p.TrackingNumber,
                    Status = p.Status,
                    Size = p.Size,
                    WeightKg = p.WeightKg,
                    ScheduledPickupDate = p.ScheduledPickupDate,


                    SenderUser = p.SenderUser == null ? null : new()
                    {
                        FullName = p.SenderUser.FullName,
                        Email = p.SenderUser.Email
                    },
                    SenderName = p.SenderName,
                    SenderEmail = p.SenderEmail,
                    SenderPhone = p.SenderPhone,
                    SenderAddress = new()
                    {
                        Country = p.SenderAddress.Country,
                        State = p.SenderAddress.State,
                        PostalCode = p.SenderAddress.PostalCode,
                        City = p.SenderAddress.City,
                        Street = p.SenderAddress.Street,
                        StreetNumber = p.SenderAddress.StreetNumber,
                        BuildingDetails = p.SenderAddress.BuildingDetails
                    },


                    RecipientUser = p.RecipientUser == null ? null : new()
                    {
                        FullName = p.RecipientUser.FullName,
                        Email = p.RecipientUser.Email
                    },
                    RecipientName = p.RecipientName,
                    RecipientEmail = p.RecipientEmail,
                    RecipientPhone = p.RecipientPhone,
                    DeliveryAddress = new()
                    {
                        Country = p.DeliveryAddress.Country,
                        State = p.DeliveryAddress.State,
                        PostalCode = p.DeliveryAddress.PostalCode,
                        City = p.DeliveryAddress.City,
                        Street = p.DeliveryAddress.Street,
                        StreetNumber = p.DeliveryAddress.StreetNumber,
                        BuildingDetails = p.DeliveryAddress.BuildingDetails
                    },


                    PickupCourier = p.PickupCourier == null ? null : new()
                    {
                        FullName = p.PickupCourier.FullName,
                        Email = p.PickupCourier.Email
                    },

                    DeliveryCourier = p.DeliveryCourier == null ? null : new()
                    {
                        FullName = p.DeliveryCourier.FullName,
                        Email = p.DeliveryCourier.Email
                    },

                    Warehouse = p.Warehouse == null ? null : new()
                    {
                        Name = p.Warehouse.Name
                    },


                    CreatedAt = p.CreatedAt,
                    PickedUpAt = p.PickedUpAt,
                    ReceivedInWarehouseAt = p.ReceivedInWarehouseAt,
                    OutForDeliveryAt = p.OutForDeliveryAt,
                    DeliveredAt = p.DeliveredAt
                })
                .ToListAsync();
        }


        public async Task<bool> RecordPickupAsync(string trackingNumber, int courierId)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.",
                    nameof(trackingNumber));

            if (courierId <= 0)
                throw new ArgumentOutOfRangeException(nameof(courierId));

            var package = await _dbContext.Packages
                .FirstOrDefaultAsync(p =>
                    p.TrackingNumber == trackingNumber &&
                    p.PickupCourierId == courierId);

            // If package does not exist, or is not assigned to this courier.
            if (package == null)
                return false;

            // Only 'PickupAssigned' status is allowed.
            if (package.Status != Shared.Enums.PackageStatus.PickupAssigned)
                return false;

            var now = DateTime.UtcNow;

            package.Status = Shared.Enums.PackageStatus.PickedUp;
            package.PickedUpAt = now;

            var history = new PackageStatusHistory
            {
                PackageId = package.Id,
                Status = Shared.Enums.PackageStatus.PickedUp,
                Timestamp = now,
                ChangedByUserId = courierId,
                Note = "The courier successfully recorded the package pickup."
            };

            _dbContext.PackageStatusHistories.Add(history);

            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<bool> RecordOutForDeliveryAsync(string trackingNumber, int courierId)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.",
                    nameof(trackingNumber));

            if (courierId <= 0)
                throw new ArgumentOutOfRangeException(nameof(courierId));

            var package = await _dbContext.Packages
                .FirstOrDefaultAsync(p =>
                    p.TrackingNumber == trackingNumber &&
                    p.DeliveryCourierId == courierId);

            // If package does not exist, or is not assigned to this courier.
            if (package == null)
                return false;

            // Only 'DeliveryAssigned' status is allowed.
            if (package.Status != Shared.Enums.PackageStatus.DeliveryAssigned)
                return false;

            var now = DateTime.UtcNow;

            package.Status = Shared.Enums.PackageStatus.OutForDelivery;
            package.PickedUpAt = now;

            var history = new PackageStatusHistory
            {
                PackageId = package.Id,
                Status = Shared.Enums.PackageStatus.OutForDelivery,
                Timestamp = now,
                ChangedByUserId = courierId,
                Note = "The courier successfully recorded the package as out for delivery."
            };

            _dbContext.PackageStatusHistories.Add(history);

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
