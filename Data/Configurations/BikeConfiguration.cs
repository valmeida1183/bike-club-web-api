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
                .HasColumnType("decimal(3,1)")                
                .IsRequired();
            builder.Property(b => b.RimSize)
                .HasColumnType("decimal(3,1)")               
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
            builder.Property(b => b.Image)
                .HasMaxLength(50)
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