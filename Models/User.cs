using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [EmailAddress(ErrorMessage = "{0} field is not a valid email")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [RegularExpression(@"^(?:\()[0-9]{2}(?:\))\s?[0-9]{4,5}(?:-)[0-9]{4}$", ErrorMessage = "{0} field is not a valid phone number")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(35, MinimumLength = 1, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string LastName { get; set; }

        public string GenderCode { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<TourParticipant> TourParticipants { get; set; }
        public virtual Role Role { get; set; }
        public virtual Gender gender {get; set;}
    }
}