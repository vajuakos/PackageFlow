using PackageFlow.Shared.Enums;
using PackageFlow.Shared.DTOs.Common;

namespace PackageFlow.Shared.DTOs.WarehouseManager
{
    public class WarehousePackageResult
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public PackageStatus Status { get; set; }
        public PackageSize Size { get; set; }
        public decimal WeightKg { get; set; }
        public DateOnly ScheduledPickupDate { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public AddressResult DeliveryAddress { get; set; } = new();
        public int? PickupCourierId { get; set; }
        public string? PickupCourierName { get; set; }
        public int? DeliveryCourierId { get; set; }
        public string? DeliveryCourierName { get; set; }
        public DateTime? ReceivedInWarehouseAt { get; set; }
    }
}