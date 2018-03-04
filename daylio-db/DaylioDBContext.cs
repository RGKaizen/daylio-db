using rgkaizen.daylio.Models;
using Microsoft.EntityFrameworkCore;

namespace rgkaizen.daylio
{
    public class DaylioDBContext : DbContext
    {

        public DaylioDBContext(DbContextOptions<DaylioDBContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    
        public DbSet<DaylioActivityEntryRefModel> ActivityEntryRefs { get; set; }

        public DbSet<DaylioActivityModel> Activities { get; set; }

        public DbSet<DaylioEntryModel> Entries { get; set; }

        public DbSet<DaylioRawModel> Raws { get; set; }
    }
}
