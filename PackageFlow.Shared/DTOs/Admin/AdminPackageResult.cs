using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.Enums;

namespace PackageFlow.Shared.DTOs.Admin
{
    public class AdminPackageResult
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public PackageStatus Status { get; set; }
        public PackageSize Size { get; set; }
        public decimal WeightKg { get; set; }
        public DateOnly ScheduledPickupDate { get; set; }

        // Sender
        public int? SenderUserId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPhone { get; set; } = string.Empty;
        public AddressResult SenderAddress { get; set; } = new();

        // Recipient
        public int? RecipientUserId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string RecipientPhone { get; set; } = string.Empty;
        public AddressResult DeliveryAddress { get; set; } = new();

        // Couriers
        public int? PickupCourierId { get; set; }
        public string? PickupCourierName { get; set; }
        public int? DeliveryCourierId { get; set; }
        public string? DeliveryCourierName { get; set; }

        // Warehouse
        public string? WarehouseName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? ReceivedInWarehouseAt { get; set; }
        public DateTime? OutForDeliveryAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
