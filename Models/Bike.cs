using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class Bike
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [Range(0, 36, ErrorMessage = "{0} field must contain a value between {2} and {1}")]
        public int Gears { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [Range(13, 24, ErrorMessage = "{0} field must contain a value between {2} and {1}")]
        public decimal FrameSize { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [Range(12, 29, ErrorMessage = "{0} field must contain a value between {2} and {1}")]
        public decimal RimSize { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Model { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [DataType(DataType.Currency)]
        [Range(0.0, Double.MaxValue, ErrorMessage = "{0} field must be greater than {1}")]
        public decimal Price { get; set; }        
        
        public string GenderCode { get; set; }
        public int CategoryId { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }

        public virtual Gender Gender { get; set; }
        public virtual Category Category {get; set; }
    }
}