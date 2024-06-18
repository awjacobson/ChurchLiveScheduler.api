using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.api.Repositories;
using ChurchLiveScheduler.api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var connectionString = Environment.GetEnvironmentVariable("DefaultConnection", EnvironmentVariableTarget.Process);

        services.AddDbContext<SchedulerDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddScoped<ISchedulerService, SchedulerService>();
        services.AddScoped<ISpecialsRepository, SpecialsRepository>();
        services.AddScoped<ISeriesRepository, SeriesRepository>();
        services.AddScoped<ICancellationsRepository, CancellationsRepository>();
    })
    .Build();

host.Run();