using Microsoft.EntityFrameworkCore;
using PackageFlow.Core.DTOs;
using PackageFlow.Core.Enums;
using PackageFlow.Core.Helpers;
using PackageFlow.Core.Models;
using PackageFlow.Data.Context;

namespace PackageFlow.Data.Services.PackageHandler
{
    public class PackageHandlerService : IPackageHandlerService
    {
        private readonly AppDbContext _dbContext;

        public PackageHandlerService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void CreatePackage(CreatePackageRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var trackingNumber = TrackingNumberHelper.Generate();

            var package = new Package
            {
                TrackingNumber = trackingNumber,
                Status = PackageStatus.Registered,
                Size = request.SizeCategory,
                WeightKg = request.WeightKg,
                ScheduledPickupDate = DateOnly.FromDateTime(request.ScheduledPickupDate),
                CreatedAt = DateTime.UtcNow,

                SenderUserId = request.SenderUserId,
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
                Note = "Csomag előjegyezve a rendszerben."
            });

            _dbContext.Packages.Add(package);
            _dbContext.SaveChanges();
        }

        public Package? GetPackageByTrackingNumber(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber)) return null;

            var cleanTrackingNumber = trackingNumber.Trim();

            return _dbContext.Packages
                .Include(p => p.StatusHistory)
                .AsNoTracking() // disables EF change tracking for better performance
                .FirstOrDefault(p => p.TrackingNumber == cleanTrackingNumber);
        }
    }
}
