namespace BikeClub.Domain.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string? Street { get; set; }
        public string? Complement { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public int ZipCode { get; set; }
    }
}
