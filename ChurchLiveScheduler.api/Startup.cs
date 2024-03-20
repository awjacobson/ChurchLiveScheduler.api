using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.api.Repositories;
using ChurchLiveScheduler.api.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ChurchLiveScheduler.api.Startup))]

namespace ChurchLiveScheduler.api;

/// <summary>
/// https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
/// </summary>
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddDbContext<SchedulerDbContext>(options =>
        {
            options.UseSqlite("Data Source=schedule.db");
        });

        builder.Services.AddScoped<ISchedulerService, SchedulerService>();
        builder.Services.AddScoped<ISpecialsRepository, SpecialsRepository>();
        builder.Services.AddScoped<ISeriesRepository, SeriesRepository>();
    }
}
