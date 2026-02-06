using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [EmailAddress(ErrorMessage = "{0} field is not a valid email")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} field is required")]
        [RegularExpression(@"^(?:\()[0-9]{2}(?:\))\s?[0-9]{4,5}(?:-)[0-9]{4}$", ErrorMessage = "{0} field is not a valid phone number")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string LastName { get; set; } = string.Empty;

        public string GenderCode { get; set; } = string.Empty;
        public string? RoleName { get; set; } = string.Empty;
        public virtual ICollection<TourParticipant>? TourParticipants { get; set; }
        public virtual Role? Role { get; set; }
        public virtual Gender? Gender { get; set; }
        public virtual ShopCart? ShopCart { get; set; }
    }
}