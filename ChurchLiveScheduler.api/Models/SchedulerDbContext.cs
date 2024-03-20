using Microsoft.EntityFrameworkCore;
using System;

namespace ChurchLiveScheduler.api.Models;

/// <summary>
/// https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=visual-studio
/// </summary>
public class SchedulerDbContext : DbContext
{
    public DbSet<Series> Series { get; set; }
    public DbSet<Special> Specials { get; set; }

    //public string DbPath { get; }

    //public SchedulerDbContext()
    //{
    //    //var folder = Environment.SpecialFolder.LocalApplicationData;
    //    //var path = Environment.GetFolderPath(folder);
    //    //DbPath = System.IO.Path.Join(path, "shcedule.db");
    //    DbPath = "schedule.db";
    //}

    //// The following configures EF to create a Sqlite database file in the
    //// special "local" folder for your platform.
    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //    => options.UseSqlite($"Data Source={DbPath}");

    public SchedulerDbContext(DbContextOptions<SchedulerDbContext> options) : base(options)
    {
    }
}
