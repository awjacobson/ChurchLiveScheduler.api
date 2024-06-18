using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ChurchLiveScheduler.api.Services;
using System.IO;
using System.Text.Json;
using ChurchLiveScheduler.api.Models;

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

    [FunctionName(nameof(GetSeriesList))]
    public async Task<IActionResult> GetSeriesList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("GetSeriesList");
        var seriesList = await _schedulerService.GetSeriesAsync();
        return new OkObjectResult(seriesList);
    }

    [FunctionName(nameof(GetSeriesDetail))]
    public async Task<IActionResult> GetSeriesDetail(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/{seriesId:int}")] HttpRequest req,
    int seriesId,
    ILogger log)
    {
        log.LogInformation("GetSeriesDetail (seriesId={1})", seriesId);
        var seriesDetail = await _schedulerService.GetSeriesDetailAsync(seriesId);
        return new OkObjectResult(seriesDetail);
    }

    [FunctionName(nameof(GetCancellationList))]
    public async Task<IActionResult> GetCancellationList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/{seriesId:int}/cancellations")] HttpRequest req,
        int seriesId,
        ILogger log)
    {
        log.LogInformation("GetCancellationList (seriesId={1})", seriesId);
        var cancellationList = await _schedulerService.GetCancellationsAsync(seriesId);
        return new OkObjectResult(cancellationList);
    }

    [FunctionName(nameof(CreateCancellation))]
    public async Task<IActionResult> CreateCancellation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "series/{seriesId:int}/cancellations")] HttpRequest req,
        int seriesId,
        ILogger log)
    {
        log.LogInformation("CreateCancellation (seriesId={1})", seriesId);
        var requestBody = new StreamReader(req.Body).ReadToEnd();
        var cancellationRequest = JsonSerializer.Deserialize<CreateCancellationRequest>(requestBody);
        var date = DateOnly.Parse(cancellationRequest.Date);
        var reason = cancellationRequest.Reason;
        var created = await _schedulerService.CreateCancellationAsync(seriesId, date, reason);
        return new OkObjectResult(created);
    }

    [FunctionName(nameof(UpdateCancellation))]
    public async Task<IActionResult> UpdateCancellation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "series/{seriesId:int}/cancellations/{cancellationId:int}")] HttpRequest req,
        int seriesId,
        int cancellationId,
        ILogger log)
    {
        log.LogInformation("UpdateCancellation (seriesId={1}, cancellationId={2})", seriesId, cancellationId);
        var requestBody = new StreamReader(req.Body).ReadToEnd();
        var cancellationRequest = JsonSerializer.Deserialize<CreateCancellationRequest>(requestBody);
        var date = DateOnly.Parse(cancellationRequest.Date);
        var reason = cancellationRequest.Reason;
        var updated = await _schedulerService.UpdateCancellationAsync(seriesId, cancellationId, date, reason);
        return new OkObjectResult(updated);
    }

    [FunctionName(nameof(DeleteCancellation))]
    public async Task<IActionResult> DeleteCancellation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "series/{seriesId:int}/cancellations/{cancellationId:int}")] HttpRequest req,
        int seriesId,
        int cancellationId,
        ILogger log)
    {
        log.LogInformation("DeleteCancellation (seriesId={1}, cancellationId={2})", seriesId, cancellationId);
        var deleted = await _schedulerService.DeleteCancellationAsync(seriesId, cancellationId);
        return new OkObjectResult(deleted);
    }
}
