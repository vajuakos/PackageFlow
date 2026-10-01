using PackageFlow.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace PackageFlow.Server.Models
{
    public class PackageStatusHistory
    {
        public int Id { get; set; }

        public int PackageId { get; set; }
        public Package Package { get; set; } = null!;

        public PackageStatus Status { get; set; }
        public DateTime Timestamp { get; set; }

        public int? ChangedByUserId { get; set; }
        public AppUser? ChangedByUser { get; set; }

        [MaxLength(250)]
        public string? Note { get; set; }
    }
}
