using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options)
        { 

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Register> Registers { get; set; }

        public DbSet<EmployeeViewModel>Employees { get; set; }
    }
}
