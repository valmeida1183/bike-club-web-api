using BikeClub.Data.Extensions;
using BikeClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data
{
    public class DataContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public DataContext(DbContextOptions<DataContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // adicionado arquivos de configuração individualmente
            //modelBuilder.ApplyConfiguration(new RoleConfiguration());

            // vai ler todos os arquivos que herdam de IEntityTypeConfiguration do assembly e registrá-los de uma única vez.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

            //Seed do banco de dados
            modelBuilder.Seed();
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Bike> Bikes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ShopCart> ShopCarts { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourParticipant> TourParticipants { get; set; }
        public DbSet<User> Users { get; set; }
    }
}