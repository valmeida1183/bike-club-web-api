using BikeClub.Models;
using BikeClub.Services;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data.Seed
{
    public static class UserSeedConfiguration
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Email = "administrator@administrator.com", Password = CryptographerService.Hash("Master1"), Phone = "(99)99999-9999", Name = "Admin", LastName = "Master", GenderCode = "M", RoleName = "Monitor" }
            );
        }
    }
}