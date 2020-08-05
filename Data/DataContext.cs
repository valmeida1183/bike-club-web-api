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
            // adicionado arquivos de configuração individualmente
            //modelBuilder.ApplyConfiguration(new RoleConfiguration());

            // vai ler todos os arquivos que herdam de IEntityTypeConfiguration do assembly e registrá-los de uma única vez.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }

        public DbSet<Address> Addresses { get; set;}
        public DbSet<Category> Categories { get; set; }
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourParticipant> TourParticipants { get; set; }
        public DbSet<User> Users {get; set; }  
    }
}