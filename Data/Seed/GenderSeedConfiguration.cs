using BikeClub.Models;
using BikeClub.Static;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data.Seed
{
    public static class GenderSeedConfiguration
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gender>().HasData(
                new Gender { Code = GenderStatic.Male, Description = "Male"},
                new Gender { Code = GenderStatic.Female, Description = "Female"}
            );
        }
    }
}