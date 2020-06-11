using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class Gender 
    {   [Required(ErrorMessage = "{0} field is required")]
        [MaxLength(1, ErrorMessage = "{0} field must contain a maximum of {1} characters ")]        
        public string Code { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Description { get; set; }
    }
}