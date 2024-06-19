using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.api.Repositories;
using ChurchLiveScheduler.api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string DevEnvironmentValue = "Development";
const string DBPath = "schedule.db";
const string Azure_DBPath = "D:/home/schedule.db";

static void CopyDb()
{
    File.Copy(DBPath, Azure_DBPath);
    File.SetAttributes(Azure_DBPath, FileAttributes.Normal);
}

bool isDevelopmentEnvironment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == DevEnvironmentValue;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        if (!isDevelopmentEnvironment && !File.Exists(Azure_DBPath))
        {
            CopyDb();
        }

        var connectionString = $"Data Source={(isDevelopmentEnvironment ? DBPath : Azure_DBPath)};";

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