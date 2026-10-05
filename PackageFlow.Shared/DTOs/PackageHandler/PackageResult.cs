using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.Enums;

namespace PackageFlow.Shared.DTOs.PackageHandler
{
    public class PackageResult
    {
        public string TrackingNumber { get; set; }
        public PackageStatus Status { get; set; }
        public PackageSize Size { get; set; }
        public decimal WeightKg { get; set; }

        public DateOnly ScheduledPickupDate { get; set; }

        // Sender details
        public UserResult? SenderUser { get; set; }

        // Details in case of Guest user
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPhone { get; set; }
        public AddressResult SenderAddress { get; set; } = new();

        // Recipient
        public UserResult? RecipientUser { get; set; }

        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientPhone { get; set; }
        public AddressResult DeliveryAddress { get; set; } = new();

        // Assigned couriers
        public UserResult? PickupCourier { get; set; }

        public UserResult? DeliveryCourier { get; set; }

        // Warehouse
        public WarehouseResult? Warehouse { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? ReceivedInWarehouseAt { get; set; }
        public DateTime? OutForDeliveryAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
