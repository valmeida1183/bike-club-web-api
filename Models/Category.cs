using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Name { get; set; }
    }
}