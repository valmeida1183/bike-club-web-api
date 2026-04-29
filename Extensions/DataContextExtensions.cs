using BikeClub.Data;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Extensions;

public static class DataContextExtensions
{
    public static WebApplicationBuilder ConfigureDataContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(connectionString));
        return builder;
    }
}
