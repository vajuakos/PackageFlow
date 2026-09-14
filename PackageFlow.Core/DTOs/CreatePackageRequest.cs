using PackageFlow.Core.Enums;

namespace PackageFlow.Core.DTOs
{
    public class CreatePackageRequest
    {
        public int? SenderUserId { get; set; }

        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPhone { get; set; }
        public string SenderCountry { get; set; }
        public string SenderState { get; set; }
        public int SenderPostalCode { get; set; }
        public string SenderCity { get; set; }
        public string SenderStreet { get; set; }
        public int SenderStreetNumber { get; set; }
        public string? SenderBuildingDetails { get; set; }

        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientPhone { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryState { get; set; }
        public int DeliveryPostalCode { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryStreet { get; set; }
        public int DeliveryStreetNumber { get; set; }
        public string? DeliveryBuildingDetails { get; set; }

        public PackageSize SizeCategory { get; set; }
        public decimal WeightKg { get; set; }
        public DateTime ScheduledPickupDate { get; set; } = DateTime.Now.AddDays(1);
        public string? PickupTimeSlot { get; set; }
    }
}
