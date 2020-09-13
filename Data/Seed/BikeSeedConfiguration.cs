using BikeClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Data.Seed
{
    public static class BikeSeedConfiguration
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bike>().HasData(
                new Bike { Id = 1, Gears = 18, FrameSize = 17.5m, RimSize = 27.5m, Model= "Silver Mountain", Description = "The classic mountain bike. Equipped with front suspension, V-Brake and aluminum frame.", Price = 600.90m, Image = "mountain1.png", GenderCode = "M", CategoryId = 2 },
                new Bike { Id = 2, Gears = 10, FrameSize = 17.5m, RimSize = 27.5m, Model= "Mechanical Orange", Description = "Versatile model, classified as hybrid. Equipped with front suspension, V-Brake, aluminum frame.", Price = 450.90m, Image = "mountain2.png", GenderCode = "M", CategoryId = 3 },
                new Bike { Id = 3, Gears = 21, FrameSize = 17.5m, RimSize = 27.5m, Model= "Blue Sky", Description = "Advanced hybrid model. Equipped with front suspension, disc brakes, aluminum frame.", Price = 1200.90m, Image = "mountain3.png", GenderCode = "M", CategoryId = 3 },
                new Bike { Id = 4, Gears = 21, FrameSize = 17.5m, RimSize = 29.0m, Model= "Snow Mountain", Description = "The winter bike. Equipped with larger, non-slip special tires, disc brakes, aluminum frame.", Price = 1400.90m, Image = "mountain4.png", GenderCode = "M", CategoryId = 2 },
                new Bike { Id = 5, Gears = 18, FrameSize = 17.5m, RimSize = 27.5m, Model= "Orange Tour", Description = "The classic bicycle for tours. Equipped with V-Brake brakes, aluminum frame, and rear and front load support.", Price = 380.90m, Image = "tour1.png", GenderCode = "M", CategoryId = 4 },
                new Bike { Id = 6, Gears = 21, FrameSize = 17.5m, RimSize = 29.0m, Model= "Silver Mountain II", Description = "Silver mountain's advanced model. Equipped with RockShox front suspension, disc brakes, carbon fiber frame.", Price = 2390.90m, Image = "mountain5.png", GenderCode = "M", CategoryId = 2 },
                new Bike { Id = 7, Gears = 36, FrameSize = 17.5m, RimSize = 29.0m, Model= "Scarlet Mountain", Description = "The top of mountain bikes. Equipped with RockShox front suspension, disc brakes, carbon fiber frame and Maxxis Minion tires.", Price = 5790.90m, Image = "mountain6.png", GenderCode = "M", CategoryId = 2 },
                new Bike { Id = 8, Gears = 10, FrameSize = 17.5m, RimSize = 27.5m, Model= "Orange Speed", Description = "Classic road model. Equipped with V-brake and aluminum frame.", Price = 350.00m, Image = "road1.png", GenderCode = "M", CategoryId = 1 },
                new Bike { Id = 9, Gears = 21, FrameSize = 17.5m, RimSize = 29.0m, Model= "Scarlet Speed", Description = "The top of road bikes. Equipped with disc brakes, carbon fiber frame and thin tires.", Price = 6100.90m, Image = "road2.png", GenderCode = "M", CategoryId = 1 },
                new Bike { Id = 10, Gears = 18, FrameSize = 17.5m, RimSize = 29.0m, Model= "Yellow Speed", Description = "Advanced road bike model. Equipped with disc brakes and aluminum frame.", Price = 2200.90m, Image = "road3.png", GenderCode = "M", CategoryId = 1 },
                new Bike { Id = 11, Gears = 0, FrameSize = 17.5m, RimSize = 27.5m, Model= "Green Speed", Description = "Root road model. Equipped with V-Brake and aluminum frame.", Price = 500.0m, Image = "road4.png", GenderCode = "M", CategoryId = 1 },
                new Bike { Id = 12, Gears = 0, FrameSize = 17.5m, RimSize = 29.0m, Model= "Blue Speed", Description = "Root road model. Equipped with V-Brake and aluminum frame.", Price = 500.0m, Image = "road5.png", GenderCode = "M", CategoryId = 1 },
                new Bike { Id = 13, Gears = 36, FrameSize = 17.5m, RimSize = 29.0m, Model= "Black Widow", Description = "Special and exclusive model of Road bike. Equipped with disc brakes, carbon fiber frame and thin tires.", Price = 9000.0m, Image = "road6.png", GenderCode = "M", CategoryId = 1 },
                new Bike { Id = 14, Gears = 18, FrameSize = 17.5m, RimSize = 27.5m, Model= "Gray Tour", Description = "More versatile hybrid bike. Equipped with V-Brake brakes and aluminum frame", Price = 450.0m, Image = "tour2.png", GenderCode = "M", CategoryId = 3 },
                new Bike { Id = 15, Gears = 0, FrameSize = 17.5m, RimSize = 27.5m, Model= "Green Tour", Description = "Root tour bike. Equipped with V-Brake brakes, aluminum frame and rear load support", Price = 550.0m, Image = "tour3.png", GenderCode = "M", CategoryId = 4 },
                new Bike { Id = 16, Gears = 18, FrameSize = 17.5m, RimSize = 29.9m, Model= "Green Tour II", Description = "Advanced tour bike. Equipped with V-Brake brakes and carbon fiber frame", Price = 950.0m, Image = "tour4.png", GenderCode = "M", CategoryId = 4 },
                new Bike { Id = 17, Gears = 21, FrameSize = 17.5m, RimSize = 27.5m, Model= "Green Eletric", Description = "Eletric tour bike. Equipped with V-Brake brakes and carbon fiber frame, powerfull front light and rear load support", Price = 3000.0m, Image = "tour5.png", GenderCode = "M", CategoryId = 7 },
                new Bike { Id = 18, Gears = 0, FrameSize = 17.5m, RimSize = 27.5m, Model= "Orange Eletric", Description = "Eletric tour bike. Equipped with V-Brake brakes and carbon fiber frame, powerfull front light, rear and front load support", Price = 3200.0m, Image = "tour6.png", GenderCode = "F", CategoryId = 7 },
                new Bike { Id = 19, Gears = 0, FrameSize = 17.5m, RimSize = 29.0m, Model= "Blue Tour", Description = "Root tour bike. Equipped with V-Brake brakes and aluminum frame, rear and front load support", Price = 600.0m, Image = "tour7.png", GenderCode = "M", CategoryId = 4 },
                new Bike { Id = 20, Gears = 6, FrameSize = 17.5m, RimSize = 27.5m, Model= "Pink Tour", Description = "Special tour bike. Equipped with V-Brake brakes and aluminum frame, rear and front load support", Price = 400.0m, Image = "tour8.png", GenderCode = "F", CategoryId = 4 }
            );
        }
    }
}