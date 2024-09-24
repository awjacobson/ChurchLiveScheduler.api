using ChurchLiveScheduler.api.Services;
using ChurchLiveScheduler.sdk.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChurchLiveScheduler.api;

public class ChurchLiveSchedulerFunction(ISchedulerService schedulerService, ILogger<ChurchLiveSchedulerFunction> logger)
{
    private readonly ILogger<ChurchLiveSchedulerFunction> _logger = logger;
    private readonly ISchedulerService _schedulerService = schedulerService;

    [Function(nameof(GetCurrentTime))]
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

        string? queryDate = req.Query["date"];

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

    #region Series
    [Function(nameof(GetSeriesList))]
    public async Task<IActionResult> GetSeriesList(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "series")] HttpRequest req)
    {
        _logger.LogInformation("GetSeriesList");
        var seriesList = await _schedulerService.GetSeriesAsync();
        return new OkObjectResult(seriesList);
    }

    [Function(nameof(GetSeriesDetail))]
    public async Task<IActionResult> GetSeriesDetail(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "series/{seriesId:int}")] HttpRequest req,
        int seriesId)
    {
        _logger.LogInformation("GetSeriesDetail (seriesId={SeriesId})", seriesId);
        var seriesDetail = await _schedulerService.GetSeriesDetailAsync(seriesId);
        return new OkObjectResult(seriesDetail);
    }

    [Function(nameof(UpdateSeries))]
    public async Task<IActionResult> UpdateSeries(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "series/{id:int}")] HttpRequest req,
        int id)
    {
        _logger.LogInformation("UpdateSeries (seriesId={SeriesId})", id);
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var request = JsonSerializer.Deserialize<UpdateSeriesRequest>(requestBody);
        var response = await _schedulerService.UpdateSeriesAsync(id, request);
        return new OkObjectResult(response);
    }
    #endregion

    #region Cancellations
    [Function(nameof(GetCancellationList))]
    public async Task<IActionResult> GetCancellationList(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "series/{seriesId:int}/cancellations")] HttpRequest req,
        int seriesId)
    {
        _logger.LogInformation("GetCancellationList (seriesId={SeriesId})", seriesId);
        var cancellationList = await _schedulerService.GetCancellationsAsync(seriesId);
        return new OkObjectResult(cancellationList);
    }

    [Function(nameof(CreateCancellation))]
    public async Task<IActionResult> CreateCancellation(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "series/{seriesId:int}/cancellations")] HttpRequest req,
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
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "series/{seriesId:int}/cancellations/{cancellationId:int}")] HttpRequest req,
        int seriesId,
        int cancellationId)
    {
        _logger.LogInformation("UpdateCancellation (seriesId={SeriesId}, cancellationId={CancellationId})", seriesId, cancellationId);
        var requestBody = new StreamReader(req.Body).ReadToEnd();
        var request = JsonSerializer.Deserialize<UpdateCancellationRequest>(requestBody);
        var updated = await _schedulerService.UpdateCancellationAsync(seriesId, cancellationId, request);
        return new OkObjectResult(updated);
    }

    [Function(nameof(DeleteCancellation))]
    public async Task<IActionResult> DeleteCancellation(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "series/{seriesId:int}/cancellations/{cancellationId:int}")] HttpRequest req,
        int seriesId,
        int cancellationId)
    {
        _logger.LogInformation("DeleteCancellation (seriesId={SeriesId}, cancellationId={CancellationId})", seriesId, cancellationId);
        var response = await _schedulerService.DeleteCancellationAsync(seriesId, cancellationId);
        return new OkObjectResult(response);
    }
    #endregion

    #region Specials
    [Function(nameof(GetSpecials))]
    public async Task<IActionResult> GetSpecials([HttpTrigger(AuthorizationLevel.Function, "get", Route = "specials")] HttpRequest req)
    {
        _logger.LogInformation("GetSpecials");
        var response = await _schedulerService.GetSpecialsAsync();
        return new OkObjectResult(response);
    }

    [Function(nameof(CreateSpecial))]
    public async Task<IActionResult> CreateSpecial(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "specials")] HttpRequest req)
    {
        _logger.LogInformation("CreateSpecial");
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var request = JsonSerializer.Deserialize<CreateSpecialRequest>(requestBody);

        if (request == null)
        {
            return new BadRequestObjectResult("request is null");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new BadRequestObjectResult("Name is null");
        }

        if (string.IsNullOrWhiteSpace(request.Date))
        {
            return new BadRequestObjectResult("Date is null");
        }

        var response = await _schedulerService.CreateSpecial(request);
        return new OkObjectResult(response);
    }

    [Function(nameof(UpdateSpecial))]
    public async Task<IActionResult> UpdateSpecial(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "specials/{id:int}")] HttpRequest req,
        int id)
    {
        _logger.LogInformation("UpdateSpecial (id={id})", id);
        var requestBody = new StreamReader(req.Body).ReadToEnd();
        var request = JsonSerializer.Deserialize<UpdateSpecialRequest>(requestBody);
        var response = await _schedulerService.UpdateSpecialAsync(id, request);
        return new OkObjectResult(response);
    }

    [Function(nameof(DeleteSpecial))]
    public async Task<IActionResult> DeleteSpecial(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "specials/{id:int}")] HttpRequest req,
        int id)
    {
        _logger.LogInformation("DeleteSpecial (id={id})", id);
        var response = await _schedulerService.DeleteSpecialAsync(id);
        return new OkObjectResult(response);
    }
    #endregion
}
