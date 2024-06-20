using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChurchLiveScheduler.api;

public class ChurchLiveSchedulerFunction
{
    private readonly ILogger<ChurchLiveSchedulerFunction> _logger;
    private readonly ISchedulerService _schedulerService;

    public ChurchLiveSchedulerFunction(ISchedulerService schedulerService, ILogger<ChurchLiveSchedulerFunction> logger)
    {
        _logger = logger;
        _schedulerService = schedulerService;
    }

    [Function("GetCurrentTime")]
    public IActionResult GetCurrentTime(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("GetNext");

        var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        var date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralTimeZone);

        return new OkObjectResult(date);
    }

    [Function(nameof(GetNext))]
    public async Task<IActionResult> GetNext(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("GetNext");

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

    [Function(nameof(GetAll))]
    public async Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("GetAll");
        var all = await _schedulerService.GetAllAsync();
        return new OkObjectResult(all);
    }

    [Function(nameof(GetSeriesList))]
    public async Task<IActionResult> GetSeriesList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series")] HttpRequest req)
    {
        _logger.LogInformation("GetSeriesList");
        var seriesList = await _schedulerService.GetSeriesAsync();
        return new OkObjectResult(seriesList);
    }

    [Function(nameof(GetSeriesDetail))]
    public async Task<IActionResult> GetSeriesDetail(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/{seriesId:int}")] HttpRequest req,
    int seriesId)
    {
        _logger.LogInformation("GetSeriesDetail (seriesId={SeriesId})", seriesId);
        var seriesDetail = await _schedulerService.GetSeriesDetailAsync(seriesId);
        return new OkObjectResult(seriesDetail);
    }

    [Function(nameof(GetCancellationList))]
    public async Task<IActionResult> GetCancellationList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/{seriesId:int}/cancellations")] HttpRequest req,
        int seriesId)
    {
        _logger.LogInformation("GetCancellationList (seriesId={SeriesId})", seriesId);
        var cancellationList = await _schedulerService.GetCancellationsAsync(seriesId);
        return new OkObjectResult(cancellationList);
    }

    [Function(nameof(CreateCancellation))]
    public async Task<IActionResult> CreateCancellation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "series/{seriesId:int}/cancellations")] HttpRequest req,
        int seriesId)
    {
        _logger.LogInformation("CreateCancellation (seriesId={SeriesId})", seriesId);
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var cancellationRequest = JsonSerializer.Deserialize<CreateCancellationRequest>(requestBody);
        var date = DateOnly.Parse(cancellationRequest.Date);
        var reason = cancellationRequest.Reason;
        var created = await _schedulerService.CreateCancellationAsync(seriesId, date, reason);
        return new OkObjectResult(created);
    }

    [Function(nameof(UpdateCancellation))]
    public async Task<IActionResult> UpdateCancellation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "series/{seriesId:int}/cancellations/{cancellationId:int}")] HttpRequest req,
        int seriesId,
        int cancellationId)
    {
        _logger.LogInformation("UpdateCancellation (seriesId={SeriesId}, cancellationId={CancellationId})", seriesId, cancellationId);
        var requestBody = new StreamReader(req.Body).ReadToEnd();
        var cancellationRequest = JsonSerializer.Deserialize<CreateCancellationRequest>(requestBody);
        var date = DateOnly.Parse(cancellationRequest.Date);
        var reason = cancellationRequest.Reason;
        var updated = await _schedulerService.UpdateCancellationAsync(seriesId, cancellationId, date, reason);
        return new OkObjectResult(updated);
    }

    [Function(nameof(DeleteCancellation))]
    public async Task<IActionResult> DeleteCancellation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "series/{seriesId:int}/cancellations/{cancellationId:int}")] HttpRequest req,
        int seriesId,
        int cancellationId)
    {
        _logger.LogInformation("DeleteCancellation (seriesId={SeriesId}, cancellationId={CancellationId})", seriesId, cancellationId);
        var deleted = await _schedulerService.DeleteCancellationAsync(seriesId, cancellationId);
        return new OkObjectResult(deleted);
    }
}
