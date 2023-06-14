using Microsoft.EntityFrameworkCore;
using NetCore_Task.Model;

namespace NetCore_Task.Data;
public class WebAPIContext: DbContext
{

    public WebAPIContext(DbContextOptions<WebAPIContext> options): base(options)
    { 
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<CalculationResultDistance> CalculationResult => Set<CalculationResultDistance>();
}