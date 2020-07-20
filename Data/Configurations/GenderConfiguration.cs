using BikeClub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeClub.Data.Configurations
{
    public class GenderConfiguration : IEntityTypeConfiguration<Gender>
    {
        public void Configure(EntityTypeBuilder<Gender> builder)
        {
            builder.HasKey(g => g.Code);
            builder.Property(g => g.Code).HasMaxLength(1);
            builder.Property(g => g.Description)
                .HasMaxLength(10)
                .IsRequired(); 
        }
    }
}