using System.Collections.Generic;

namespace BikeClub.Domain.Entities
{
    public class ShopCart
    {
        public int Id { get; set; }
        public DateTimeOffset PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int UserId { get; set; }
        public int? AddressId { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; } = [];
        public virtual User? User { get; set; }
        public virtual Address? Address { get; set; }
    }
}
