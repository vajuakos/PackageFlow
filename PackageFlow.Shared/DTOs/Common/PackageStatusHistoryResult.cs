using PackageFlow.Shared.Enums;

namespace PackageFlow.Shared.DTOs.Common
{
    public class PackageStatusHistoryResult
    {
        public int PackageId { get; set; }

        public PackageStatus Status { get; set; }

        public DateTime Timestamp { get; set; }

        public string? Note { get; set; }

        public UserResult? ChangedByUser { get; set; }
    }
}
