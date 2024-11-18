using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class ShopCart
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset PurchaseDate { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [DataType(DataType.Currency)]
        [Range(0.0, Double.MaxValue, ErrorMessage = "{0} field must be greater than {1}")]
        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public int AddressId { get; set; }

        public virtual ICollection<Purchase>? Purchases { get; set; }
        public virtual User? User { get; set; }
        public virtual Address? Address { get; set; }
    }
}