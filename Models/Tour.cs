using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BikeClub.Models
{
    public class Tour
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset StartDate { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset EndDate { get; set; }

        [Required(ErrorMessage = "{0} field is required")]
        [StringLength(300, MinimumLength = 3, ErrorMessage = "{0} field must contain between {2} and {1} characters")]
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