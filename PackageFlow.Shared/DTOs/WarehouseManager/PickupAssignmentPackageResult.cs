using PackageFlow.Shared.Enums;

namespace PackageFlow.Shared.DTOs.WarehouseManager
{
   public class PickupAssignmentPackageResult
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public PackageStatus Status { get; set; }
        public DateOnly ScheduledPickupDate { get; set; }
        public int? PickupCourierId { get; set; }
        public string? PickupCourierName { get; set; }
        public string SenderName { get; set; } = string.Empty;
    }
}
