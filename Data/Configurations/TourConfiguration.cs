using BikeClub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeClub.Data.Configurations
{
    public class TourConfiguration : IEntityTypeConfiguration<Tour>
    {
        public void Configure(EntityTypeBuilder<Tour> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.StartDate)
                .IsRequired();
            builder.Property(t => t.EndDate)
                .IsRequired();
            builder.Property(t => t.Description)
                .HasMaxLength(300)
                .IsRequired();
            builder.HasOne(t => t.Monitor)
                .WithMany()
                .HasForeignKey(t => t.MonitorId)
                .IsRequired();
            builder.HasOne(t => t.Difficulty)
                .WithMany()
                .HasForeignKey(t => t.DifficultyId)
                .IsRequired();
            builder.HasOne(t => t.Address)
                .WithMany()
                .HasForeignKey(t => t.AddressId)
                .IsRequired();
        }
    }
}