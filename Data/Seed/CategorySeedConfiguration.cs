using BikeClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data.Seed
{
    public static class CategorySeedConfiguration 
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Road" },
                new Category { Id = 2, Name = "Moutain" },
                new Category { Id = 3, Name = "Hybrid" },
                new Category { Id = 4, Name = "Touring" },
                new Category { Id = 5, Name = "Folding" },
                new Category { Id = 6, Name = "Women's" },
                new Category { Id = 7, Name = "Eletric" }
            );
        }
    }
}