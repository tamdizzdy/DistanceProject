using Distance_Calculate.Model;
using Microsoft.EntityFrameworkCore;

namespace Distance_Calculate.Data;

    public partial class WebAPIContext: DbContext
    {
        public WebAPIContext()
        {
            
        }
        public WebAPIContext(DbContextOptions<WebAPIContext> options): base(options)
        { 
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<CalculationResultDistance> CalculationResult => Set<CalculationResultDistance>();
    }
