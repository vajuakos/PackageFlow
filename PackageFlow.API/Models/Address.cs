#nullable disable

using System.ComponentModel.DataAnnotations;

namespace PackageFlow.Server.Models
{
    public class Address
    {
        [Required]
        public string Country { get; set; }

        [Required]
        public string PostalCode { get; set; }

        public string State { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string StreetNumber { get; set; }

        public string? BuildingDetails { get; set; }
    }
}
