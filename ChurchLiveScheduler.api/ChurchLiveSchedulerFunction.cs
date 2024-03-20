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

    [FunctionName(nameof(GetNext))]
    public async Task<IActionResult> GetNext(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        //log.LogInformation("C# HTTP trigger function processed a request.");

        string queryDate = req.Query["date"];

        //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //dynamic data = JsonConvert.DeserializeObject(requestBody);
        //name = name ?? data?.name;

        //string responseMessage = string.IsNullOrEmpty(name)
        //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        //    : $"Hello, {name}. This HTTP triggered function executed successfully.";
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
}
