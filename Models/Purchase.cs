using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class Purchase
    {
        [Required(ErrorMessage = "{0} field is required")]
        public int ShopCartId { get; set; }
        [Required(ErrorMessage = "{0} field is required")]
        public int BikeId { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} field must contain a value between {2} and {1}")]
        public int Quantity { get; set; }

        public virtual ShopCart? ShopCart { get; set; }
        public virtual Bike? Bike { get; set; }
    }
}