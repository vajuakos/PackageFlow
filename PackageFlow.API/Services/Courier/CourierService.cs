using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Data;
using PackageFlow.API.Models;
using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.DTOs.PackageHandler;
using System.Net.Security;


namespace PackageFlow.API.Services.Courier
{
    public class CourierService : ICourierService
    {
        private readonly AppDbContext _dbContext;

        public CourierService(AppDbContext dbContext)
        {
            _dbContext = dbContext
                ?? throw new ArgumentException(nameof(dbContext));
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
    }
}
