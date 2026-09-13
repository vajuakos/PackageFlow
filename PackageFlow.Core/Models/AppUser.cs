using PackageFlow.Core.Enums;

namespace PackageFlow.Core.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string VehiclePlateNumber { get; set; }
        public string DefaultAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
