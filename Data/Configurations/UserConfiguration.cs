using BikeClub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeClub.Data.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(u => u.Password) 
                .HasMaxLength(8000)               
                .IsRequired();
            builder.Property(u => u.Phone)
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(u => u.Name)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired();
            builder.HasOne(u => u.Gender)
                .WithMany()
                .HasForeignKey(u => u.GenderCode)
                .IsRequired();
            builder.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleName)
                .IsRequired();                     
        }
    }
}