using System.Collections.Generic;

namespace BikeClub.Domain.Entities
{
    public class Bike
    {
        public int Id { get; set; }
        public int Gears { get; set; }
        public decimal FrameSize { get; set; }
        public decimal RimSize { get; set; }
        public string? Model { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? GenderCode { get; set; }
        public int CategoryId { get; set; }
        public virtual ICollection<Purchase>? Purchases { get; set; }

        public virtual Gender? Gender { get; set; }
        public virtual Category? Category { get; set; }
    }
}
