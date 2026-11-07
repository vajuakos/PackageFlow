using PackageFlow.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace PackageFlow.Shared.DTOs.Admin
{
    public class AdminUpdatePackageRequest
    {
        [Required(ErrorMessage = "A státusz megadása kötelező!")]
        public PackageStatus Status { get; set; }

        [Required(ErrorMessage = "A méretkategória kiválasztása kötelező!")]
        public PackageSize Size { get; set; }

        [Range(0.1, 1000.0, ErrorMessage = "A súlynak 0.1 kg és 1000 kg között kell lennie!")]
        public decimal WeightKg { get; set; }

        [Required(ErrorMessage = "A felvétel dátumának megadása kötelező!")]
        public DateTime ScheduledPickupDate { get; set; }

        // Recipient
        [Required(ErrorMessage = "A címzett nevének megadása kötelező!")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A névnek legalább 2, legfeljebb 100 karakterből kell állnia!")]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A címzett e-mail címének megadása kötelező!")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím formátum!")]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "A címzett telefonszámának megadása kötelező!")]
        [Phone(ErrorMessage = "Érvénytelen telefonszám formátum!")]
        public string RecipientPhone { get; set; } = string.Empty;

        // Delivery address
        [Required(ErrorMessage = "Az ország megadása kötelező!")]
        public string DeliveryCountry { get; set; } = string.Empty;

        public string DeliveryState { get; set; } = string.Empty;

        [Required(ErrorMessage = "Az irányítószám megadása kötelező!")]
        public string DeliveryPostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "A város megadása kötelező!")]
        public string DeliveryCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Az utca megadása kötelező!")]
        public string DeliveryStreet { get; set; } = string.Empty;

        [Required(ErrorMessage = "A házszám megadása kötelező!")]
        public string DeliveryStreetNumber { get; set; } = string.Empty;

        public string? DeliveryBuildingDetails { get; set; }
    }
}
