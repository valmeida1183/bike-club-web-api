using System.Collections.Generic;

namespace BikeClub.Domain.Entities
{
    public class Tour
    {
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string? Description { get; set; }
        public int MonitorId { get; set; }
        public int DifficultyId { get; set; }
        public int AddressId { get; set; }

        public virtual ICollection<TourParticipant>? TourParticipants { get; set; }
        public virtual User? Monitor { get; set; }
        public virtual Difficulty? Difficulty { get; set; }
        public virtual Address? Address { get; set; }
    }
}
