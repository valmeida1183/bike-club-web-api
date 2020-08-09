using BikeClub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeClub.Data.Configurations
{
    public class BikeConfiguration : IEntityTypeConfiguration<Bike>
    {
        public void Configure(EntityTypeBuilder<Bike> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Gears)
                .HasDefaultValue(0)
                .HasMaxLength(36)
                .IsRequired();
            builder.Property(b => b.FrameSize)
                .HasMaxLength(24)
                .IsRequired();
            builder.Property(b => b.RimSize)
                .HasMaxLength(29)
                .IsRequired();
            builder.Property(b => b.Model)
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(b => b.Description)
                .HasMaxLength(300)
                .IsRequired();
            builder.Property(b => b.Price)
                .HasColumnType("money")
                .IsRequired();
            builder.HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .IsRequired();
            builder.HasOne(b => b.Gender)
                .WithMany()
                .HasForeignKey(b => b.GenderCode)
                .IsRequired();         
        }
    }
}