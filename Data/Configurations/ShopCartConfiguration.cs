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
                .WithMany()
                .HasForeignKey(s => s.UserId);
            builder.HasOne(s => s.Address)
                .WithMany()
                .HasForeignKey(s => s.AddressId);
            builder.HasMany(s => s.Purchases)
                .WithOne(p => p.ShopCart)
                .HasForeignKey(p => p.ShopCartId);
        }
    }
}