using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ChurchLiveScheduler.api.Services;

namespace ChurchLiveScheduler.api;

public class ChurchLiveSchedulerFunction
{
    private readonly ISchedulerService _schedulerService;

    public ChurchLiveSchedulerFunction(ISchedulerService schedulerService)
    {
        _schedulerService = schedulerService;
    }

    [FunctionName("GetCurrentTime")]
    public static IActionResult GetCurrentTime(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("GetNext");

        var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        var date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralTimeZone);

        return new OkObjectResult(date);
    }

    [FunctionName(nameof(GetNext))]
    public async Task<IActionResult> GetNext(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("GetNext");

        string queryDate = req.Query["date"];

        DateTime date;
        if (queryDate != null)
        {
            date = DateTime.Parse(queryDate);
        }
        else
        {
            var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralTimeZone);
        }
        var scheduledEvent = await _schedulerService.GetScheduledEventAsync(date);
        return new OkObjectResult(scheduledEvent);
    }

    [FunctionName(nameof(GetAll))]
    public async Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("GetAll");

        var all = await _schedulerService.GetAllAsync();
        return new OkObjectResult(all);

    }
}
