using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Street { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Complement { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string State { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [Display(Name = "Zip Code")]
        /*  [RegularExpression(@"^[0-9]{2}.[0-9]{3}-[0-9]{3}$", ErrorMessage="{0} field is not a valid Zip Code")] */
        public int ZipCode { get; set; }  
    }
}