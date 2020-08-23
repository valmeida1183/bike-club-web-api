using BikeClub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeClub.Data.Configurations
{
    public class TourParticipantConfiguration : IEntityTypeConfiguration<TourParticipant>
    {
        public void Configure(EntityTypeBuilder<TourParticipant> builder)
        {
            builder.HasKey(tp => new { tp.UserId, tp.TourId });
            builder.HasOne(tp => tp.User)
                .WithMany(u => u.TourParticipants)
                .HasForeignKey(tp => tp.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(tp => tp.Tour)
                .WithMany(t => t.TourParticipants)
                .HasForeignKey(tp => tp.TourId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}