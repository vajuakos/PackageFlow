namespace PackageFlow.Shared.DTOs.Common
{
    public class AddressResult
    {
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string StreetNumber { get; set; } = string.Empty;
        public string? BuildingDetails { get; set; }
    }
}
