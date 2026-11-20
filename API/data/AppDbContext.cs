using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.data
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }
    }
}
