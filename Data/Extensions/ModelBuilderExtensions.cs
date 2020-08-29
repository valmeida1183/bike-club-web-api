using BikeClub.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            CategorySeedConfiguration.Seed(modelBuilder);
            DifficultySeedConfiguration.Seed(modelBuilder);
            GenderSeedConfiguration.Seed(modelBuilder);
            RoleSeedConfiguration.Seed(modelBuilder);
        }
    }
}