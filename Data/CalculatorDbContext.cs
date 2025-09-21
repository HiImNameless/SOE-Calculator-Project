using Microsoft.EntityFrameworkCore;
using SOE_Calculator_Project.Models;

// Brandon Lombaard 223021599
// Context file for the database
namespace SOE_Calculator_Project.Data
{
    public class CalculatorDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<SavedCalculation> SavedCalculations { get; set; }

        public CalculatorDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}