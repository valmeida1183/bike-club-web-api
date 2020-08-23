using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data.Seed.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            RoleSeedConfiguration.Seed(modelBuilder);
        }
    }
}