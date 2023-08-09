using Microsoft.EntityFrameworkCore;

namespace APItest.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<Image> Images { get; set; }
    }
}
