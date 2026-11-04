using PackageFlow.Shared.Enums;
using PackageFlow.Shared.DTOs.Common;

namespace PackageFlow.Shared.DTOs.WarehouseManager
{
    public class DeliveryAssignmentPackageResult
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public PackageStatus Status { get; set; }
        public DateOnly ScheduledPickupDate { get; set; }
        public int? DeliveryCourierId { get; set; }
        public string? DeliveryCourierName { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public AddressResult DeliveryAddress { get; set; } = new();
    }
}
