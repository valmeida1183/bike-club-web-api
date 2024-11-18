
using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class Role
    {
        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string? Description { get; set; }
    }
}