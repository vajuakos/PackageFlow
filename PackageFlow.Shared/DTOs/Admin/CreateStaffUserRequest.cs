using System.ComponentModel.DataAnnotations;

namespace PackageFlow.Shared.DTOs.Admin
{
    public class CreateStaffUserRequest
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

        [Required(ErrorMessage = "A jelszó megadása kötelező!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A jelszónak legalább 6 karakterből kell állnia!")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "A szerepkör kiválasztása kötelező!")]
        public string Role { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "A rendszám legfeljebb 20 karakter lehet!")]
        public string? VehiclePlateNumber { get; set; }
    }
}
