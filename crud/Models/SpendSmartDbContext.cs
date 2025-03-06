using Microsoft.EntityFrameworkCore;

namespace crud.Models
{
    public class SpendSmartDbContext : DbContext
    {
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Auth> Authentication { get; set; }
        public SpendSmartDbContext(DbContextOptions<SpendSmartDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>()
                .Property(e => e.Value)
                .HasPrecision(10, 2);

            base.OnModelCreating(modelBuilder);
        }
    }

    
}
