using PackageFlow.Shared.Enums;

namespace PackageFlow.Shared.DTOs.PackageHandler
{
    public class CreatePackageRequest
    {
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPhone { get; set; }
        public string SenderCountry { get; set; }
        public string SenderState { get; set; }
        public string SenderPostalCode { get; set; }
        public string SenderCity { get; set; }
        public string SenderStreet { get; set; }
        public string SenderStreetNumber { get; set; }
        public string? SenderBuildingDetails { get; set; }

        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientPhone { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryState { get; set; }
        public string DeliveryPostalCode { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryStreet { get; set; }
        public string DeliveryStreetNumber { get; set; }
        public string? DeliveryBuildingDetails { get; set; }

        public PackageSize SizeCategory { get; set; }
        public decimal WeightKg { get; set; }
        public DateTime ScheduledPickupDate { get; set; } = DateTime.Now.AddDays(1);
    }
}
