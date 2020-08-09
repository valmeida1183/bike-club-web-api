using BikeClub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeClub.Data.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.HasKey(p => new { p.BikeId, p.ShopCartId });
            builder.Property(p => p.Quantity)
                .IsRequired();
            builder.HasOne(p => p.ShopCart)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.ShopCartId);
            builder.HasOne(p => p.Bike)
                .WithMany(b => b.Purchases)
                .HasForeignKey(p => p.BikeId);
        }
    }
}