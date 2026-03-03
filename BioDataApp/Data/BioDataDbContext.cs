using BioDataApp.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace BioDataApp.Data
{
    public class BioDataDbContext : DbContext
    {
        public BioDataDbContext(DbContextOptions<BioDataDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CountryTab> CountryTab { get; set; }
        public DbSet<StateTab> StateTab { get; set; }
        public DbSet<LGATab> LGATab { get; set; }

        protected BioDataDbContext()
        {
        }
    }
}
