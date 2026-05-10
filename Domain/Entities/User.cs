namespace BikeClub.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string GenderCode { get; set; } = string.Empty;
        public string? RoleName { get; set; } = string.Empty;
        public virtual ICollection<TourParticipant>? TourParticipants { get; set; }
        public virtual Role? Role { get; set; }
        public virtual Gender? Gender { get; set; }
        public virtual ShopCart? ShopCart { get; set; }
    }
}
