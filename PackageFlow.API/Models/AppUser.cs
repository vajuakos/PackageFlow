#nullable disable

using Microsoft.AspNetCore.Identity;

namespace PackageFlow.API.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public Address? DefaultAddress { get; set; }
        public DateTime CreatedAt { get; set; }

        public CourierProfile? CourierProfile { get; set; }

        public ICollection<Package> SentPackages { get; set; } = new List<Package>();
        public ICollection<Package> ReceivedPackages { get; set; } = new List<Package>();
        public ICollection<Package> PickupAssignedPackages { get; set; } = new List<Package>();
        public ICollection<Package> DeliveryAssignedPackages { get; set; } = new List<Package>();
    }
}
