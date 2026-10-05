#nullable disable

namespace PackageFlow.API.Models
{
    public class CourierProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public AppUser? User { get; set; }

        public string VehiclePlateNumber { get; set; }
    }
}
