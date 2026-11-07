using System.ComponentModel.DataAnnotations;

namespace PackageFlow.Shared.DTOs.Admin
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "A név megadása kötelező!")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A névnek legalább 2, legfeljebb 100 karakterből kell állnia!")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Az e-mail cím megadása kötelező!")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím formátum!")]
        [StringLength(100, ErrorMessage = "Az e-mail cím legfeljebb 100 karakter lehet!")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Érvénytelen telefonszám formátum!")]
        [StringLength(20, ErrorMessage = "A telefonszám legfeljebb 20 karakter lehet!")]
        public string? PhoneNumber { get; set; }

        [StringLength(20, ErrorMessage = "A rendszám legfeljebb 20 karakter lehet!")]
        public string? VehiclePlateNumber { get; set; }
    }
}
