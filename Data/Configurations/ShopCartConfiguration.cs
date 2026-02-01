using BikeClub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeClub.Data.Configurations
{
    public class ShopCartConfiguration : IEntityTypeConfiguration<ShopCart>
    {
        public void Configure(EntityTypeBuilder<ShopCart> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.PurchaseDate)
                .HasColumnType("datetimeoffset")
                .IsRequired();
            builder.Property(s => s.TotalAmount)
                .HasColumnType("money")
                .IsRequired();
            builder.HasOne(s => s.User)
                .WithOne(u => u.ShopCart)
                .HasForeignKey<ShopCart>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(s => s.Address)
                .WithMany()
                .HasForeignKey(s => s.AddressId)
                .IsRequired(false);
            builder.HasMany(s => s.Purchases)
                .WithOne(p => p.ShopCart)
                .HasForeignKey(p => p.ShopCartId);

            // Add unique constraint on UserId
            builder.HasIndex(s => s.UserId)
                .IsUnique();
        }
    }
}