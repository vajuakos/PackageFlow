using PackageFlow.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace PackageFlow.Shared.DTOs.PackageHandler
{
    public class CreatePackageRequest
    {
        // Sender details
        [Required(ErrorMessage = "A feladó nevének megadása kötelező!")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A névnek legalább 2, legfeljebb 100 karakterből kell állnia!")]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "A feladó e-mail címének megadása kötelező!")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím formátum!")]
        [StringLength(100, ErrorMessage = "Az e-mail cím legfeljebb 100 karakter lehet!")]
        public string SenderEmail { get; set; }

        [Required(ErrorMessage = "A feladó telefonszámának megadása kötelező!")]
        [Phone(ErrorMessage = "Érvénytelen telefonszám formátum!")]
        [StringLength(20, ErrorMessage = "A telefonszám legfeljebb 20 karakter lehet!")]
        public string SenderPhone { get; set; }

        [Required(ErrorMessage = "A feladó országának megadása kötelező!")]
        [StringLength(100, ErrorMessage = "Maximum 100 karakter!")]
        public string SenderCountry { get; set; }

        [Required(ErrorMessage = "A feladó megyéjének megadása kötelező!")]
        [StringLength(100, ErrorMessage = "Maximum 100 karakter!")]
        public string SenderState { get; set; }

        [Required(ErrorMessage = "A feladó irányítószámának megadása kötelező!")]
        [StringLength(20, ErrorMessage = "Maximum 20 karakter!")]
        public string SenderPostalCode { get; set; }

        [Required(ErrorMessage = "A feladó városának megadása kötelező!")]
        [StringLength(100, ErrorMessage = "Maximum 100 karakter!")]
        public string SenderCity { get; set; }

        [Required(ErrorMessage = "A feladó utcájának megadása kötelező!")]
        [StringLength(100, ErrorMessage = "Maximum 100 karakter!")]
        public string SenderStreet { get; set; }

        [Required(ErrorMessage = "A feladó házszámának megadása kötelező!")]
        [StringLength(20, ErrorMessage = "Maximum 20 karakter!")]
        public string SenderStreetNumber { get; set; }

        [StringLength(100, ErrorMessage = "Maximum 100 karakter!")]
        public string? SenderBuildingDetails { get; set; }

        // Recipient details
        [Required(ErrorMessage = "A címzett nevének megadása kötelező!")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A névnek legalább 2, legfeljebb 100 karakterből kell állnia!")]
        public string RecipientName { get; set; }

        [Required(ErrorMessage = "A címzett e-mail címének megadása kötelező!")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím formátum!")]
        [StringLength(100, ErrorMessage = "Az e-mail cím legfeljebb 100 karakter lehet!")]
        public string RecipientEmail { get; set; }

        [Required(ErrorMessage = "A címzett telefonszámának megadása kötelező!")]
        [Phone(ErrorMessage = "Érvénytelen telefonszám formátum!")]
        [StringLength(20, ErrorMessage = "A telefonszám legfeljebb 20 karakter lehet!")]
        public string RecipientPhone { get; set; }

        [Required(ErrorMessage = "A célország megadása kötelező!")]
        [StringLength(100, ErrorMessage = "Maximum 100 karakter!")]
        public string DeliveryCountry { get; set; }

        [Required(ErrorMessage = "A célmegye megadása kötelező!")]
        [StringLength(100, ErrorMessage = "Maximum 100 karakter!")]
        public string DeliveryState { get; set; }

        [Required(ErrorMessage = "A cél irányítószám megadása kötelező!")]
        [StringLength(20, ErrorMessage = "Maximum 20 karakter!")]
        public string DeliveryPostalCode { get; set; }

        [Required(ErrorMessage = "A célváros megadása kötelező!")]
        [StringLength(100, ErrorMessage = "Maximum 100 karakter!")]
        public string DeliveryCity { get; set; }

        [Required(ErrorMessage = "A célutca megadása kötelező!")]
        [StringLength(100, ErrorMessage = "Maximum 100 karakter!")]
        public string DeliveryStreet { get; set; }

        [Required(ErrorMessage = "A cél házszám megadása kötelező!")]
        [StringLength(20, ErrorMessage = "Maximum 20 karakter!")]
        public string DeliveryStreetNumber { get; set; }

        [StringLength(100, ErrorMessage = "Maximum 100 karakter!")]
        public string? DeliveryBuildingDetails { get; set; }

        // Package details
        [Required(ErrorMessage = "A méretkategória kiválasztása kötelező!")]
        public PackageSize SizeCategory { get; set; }

        [Required(ErrorMessage = "A súly megadása kötelező!")]
        [Range(0.1, 1000.0, ErrorMessage = "A súlynak 0.1 kg és 1000 kg között kell lennie!")]
        public decimal WeightKg { get; set; }

        [Required(ErrorMessage = "A felvétel dátumának megadása kötelező!")]
        public DateTime ScheduledPickupDate { get; set; } = DateTime.Now.AddDays(1);
    }
}
