using Microsoft.EntityFrameworkCore;

namespace ChurchLiveScheduler.api.Models;

public class SchedulerDbContext : DbContext
{
    public DbSet<Series> Series { get; set; }
    public DbSet<Special> Specials { get; set; }

    public SchedulerDbContext(DbContextOptions<SchedulerDbContext> options) : base(options)
    {
    }
}
