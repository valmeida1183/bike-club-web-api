using BikeClub.Models;
using BikeClub.Static;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data.Seed
{
    public static class RoleSeedConfiguration
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Name = RoleStatic.Monitor, Description = "Monitor, responsible for managing the tours" },
                new Role { Name = RoleStatic.Cyclist, Description = "Cyclist is the common user"}
            );            
        }
    }
}