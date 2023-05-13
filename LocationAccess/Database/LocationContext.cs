using LocationAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace LocationAccess.Database; 

public class LocationContext: DbContext {
    public LocationContext(DbContextOptions<LocationContext> options) : base(options)
    {
    }
        
    public DbSet<Location> Locations { get; set; } = null!;
}