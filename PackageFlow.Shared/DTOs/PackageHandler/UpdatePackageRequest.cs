using PackageFlow.Shared.Enums;

namespace PackageFlow.Shared.DTOs.PackageHandler
{
    public class UpdatePackageRequest
    {
        public DateTime ScheduledPickupDate { get; set; } = DateTime.Today.AddDays(1);
        public PackageSize Size { get; set; }
    }
}
