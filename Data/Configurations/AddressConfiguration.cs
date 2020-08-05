using BikeClub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeClub.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Street)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(a => a.Complement)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(a => a.State)
                .HasMaxLength(2)
                .IsRequired();
            builder.Property(a => a.City)
                .HasMaxLength(30)
                .IsRequired();
            builder.Property(a => a.ZipCode)
                .IsRequired();
        }
    }
}