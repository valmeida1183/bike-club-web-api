using BikeClub.Data.Configurations;
using BikeClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }

        public DbSet<Role> Roles { get; set; }
    }
}