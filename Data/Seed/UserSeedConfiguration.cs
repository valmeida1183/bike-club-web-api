using BikeClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data.Seed
{
    public static class UserSeedConfiguration
    {
        // PBKDF2-HMACSHA1 hash of "Master1" with the project SALT (10000 iterations, 32-byte output).
        // Inlined because OnModelCreating runs before DI and CryptographerService is now scoped.
        private const string MasterPasswordHash = "1lFqy5Swsz77Zh/Us7s2uMNMW+Fwhjl8PyhcDR2cpoU=";

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Email = "administrator@administrator.com", Password = MasterPasswordHash, Phone = "(99)99999-9999", Name = "Admin", LastName = "Master", GenderCode = "M", RoleName = "Monitor" }
            );
        }
    }
}
