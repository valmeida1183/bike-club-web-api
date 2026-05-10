namespace BikeClub.Domain.Entities
{
    public class Purchase
    {
        public int ShopCartId { get; set; }
        public int BikeId { get; set; }
        public int Quantity { get; set; }

        public virtual ShopCart? ShopCart { get; set; }
        public virtual Bike? Bike { get; set; }
    }
}
