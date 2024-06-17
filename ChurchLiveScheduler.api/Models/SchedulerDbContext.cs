using Microsoft.EntityFrameworkCore;

namespace ChurchLiveScheduler.api.Models;

internal sealed class SchedulerDbContext : DbContext
{
    public DbSet<Cancellation> Cancellations { get; set; }
    public DbSet<Series> Series { get; set; }
    public DbSet<Special> Specials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulerDbContext).Assembly);
    }

    public SchedulerDbContext(DbContextOptions<SchedulerDbContext> options) : base(options)
    {
    }
}
