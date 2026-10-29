using PackageFlow.Shared.Enums;

namespace PackageFlow.Shared.DTOs.Courier
{
    public class CourierShippingHistoryResult
    {
        public int PackageId { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;

        public PackageStatus Status {  get; set; }

        public DateTime Timestamp { get; set; }

        public string? Note { get; set; }

        public int? ChangedByUserId { get; set; }
    }
}
