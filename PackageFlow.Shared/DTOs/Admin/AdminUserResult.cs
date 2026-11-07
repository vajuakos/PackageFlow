namespace PackageFlow.Shared.DTOs.Admin
{
    public class AdminUserResult
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? VehiclePlateNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
