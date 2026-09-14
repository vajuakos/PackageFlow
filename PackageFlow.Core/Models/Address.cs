namespace PackageFlow.Core.Models
{
    public class Address
    {
        public string Country { get; set; }

        public int PostalCode { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public int StreetNumber { get; set; }

        public string? BuildingDetails { get; set; }
    }
}
