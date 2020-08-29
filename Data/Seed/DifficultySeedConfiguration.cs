using BikeClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data.Seed
{
    public static class DifficultySeedConfiguration
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Difficulty>().HasData(
                new Difficulty { Id = 1, Name = "Very Easy"},
                new Difficulty { Id = 2, Name = "Easy"},
                new Difficulty { Id = 3, Name = "Medium"},
                new Difficulty { Id = 4, Name = "Hard"},
                new Difficulty { Id = 5, Name = "Very Hard"}
            );
        }
    }
}