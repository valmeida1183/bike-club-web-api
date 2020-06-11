using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class Purchase
    {
        public int ShopCartId { get; set; }
        public int BikeId { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        public int Quantity { get; set; }

        public virtual ShopCart ShopCart { get; set; }
        public virtual Bike Bike { get; set; }
    }
}