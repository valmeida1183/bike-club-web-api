using BikeClub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeClub.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Name);
            builder.Property(r => r.Name).HasMaxLength(20);
            builder.Property(r => r.Description)
                .HasMaxLength(50)
                .IsRequired();                        
        }
    }
}