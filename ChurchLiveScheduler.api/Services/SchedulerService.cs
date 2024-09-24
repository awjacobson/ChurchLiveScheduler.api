using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.api.Repositories;
using ChurchLiveScheduler.sdk.Models;

namespace ChurchLiveScheduler.api.Services;

public interface ISchedulerService
{
    Task<ScheduledEvent> GetScheduledEventAsync(DateTime date);
    Task<GetAllResponse> GetAllAsync();

    Task<List<Special>> GetSpecialsAsync();
    Task<CreateSpecialResponse> CreateSpecial(CreateSpecialRequest request);
    Task<UpdateSpecialResponse> UpdateSpecialAsync(int id, UpdateSpecialRequest request);
    Task<DeleteSpecialResponse> DeleteSpecialAsync(int id);

    Task<List<Series>> GetSeriesAsync();
    Task<Series> GetSeriesDetailAsync(int id);
    Task<UpdateSeriesResponse> UpdateSeriesAsync(int id, UpdateSeriesRequest request);

    Task<List<Cancellation>> GetCancellationsAsync(int seriesId);
    Task<Cancellation> CreateCancellationAsync(int seriesId, DateOnly date, string? reason);
    Task<UpdateCancellationResponse> UpdateCancellationAsync(int seriesId, int cancellationId, UpdateCancellationRequest request);
    Task<DeleteCancellationResponse> DeleteCancellationAsync(int seriesId, int cancellationId);
}

/// <summary>
/// https://www.codeproject.com/Articles/5312004/Building-a-Micro-Web-API-with-Azure-Functions-and
/// </summary>
internal sealed class SchedulerService : ISchedulerService
{
    private readonly ISeriesRepository _seriesRepository;
    private readonly ISpecialsRepository _specialsRepository;
    private readonly ICancellationsRepository _cancellationsRepository;

    public SchedulerService(ISeriesRepository seriesRepository,
        ISpecialsRepository specialsRepository,
        ICancellationsRepository cancellationsRepository)
    {
        _seriesRepository = seriesRepository;
        _specialsRepository = specialsRepository;
        _cancellationsRepository = cancellationsRepository;
    }

    public async Task<ScheduledEvent> GetScheduledEventAsync(DateTime date)
    {
        var nextInSeries = await _seriesRepository.GetNextAsync(date);
        var nextInSpecials = await _specialsRepository.GetNextAsync(date);

        // Specials have precidence over series.
        // If a special is scheduled for the same start time as a series occurrence then pick the special.
        if (nextInSpecials != null)
        {
            if (nextInSpecials.Start <= nextInSeries.Start)
            {
                return nextInSpecials;
            }
        }

        return nextInSeries;
    }

    public async Task<GetAllResponse> GetAllAsync()
    {
        var series = await GetSeriesAsync();
        var specials = await GetSpecialsAsync();
        return new GetAllResponse
        {
            Series = series.Select(x => new SeriesDto { Id = x.Id.Value, Name = x.Name, Day = x.Day, Hours = x.Hours, Minutes = x.Minutes }),
            Specials = specials.Select(x => new SpecialDto { Id = x.Id, Date = x.Datetime, Name = x.Name })
        };
    }

    public Task<List<Special>> GetSpecialsAsync() => _specialsRepository.GetAllAsync();

    public async Task<CreateSpecialResponse> CreateSpecial(CreateSpecialRequest request)
    {
        var updated = await _specialsRepository.CreateAsync(request.Name, request.Date);
        return new CreateSpecialResponse
        {
            Id = updated.Id,
            Name = updated.Name,
            Date = updated.Datetime
        };
    }

    public async Task<UpdateSpecialResponse> UpdateSpecialAsync(int id, UpdateSpecialRequest request)
    {
        if (request?.Name is null) throw new ArgumentNullException(nameof(UpdateSpecialRequest.Name));
        if (request?.Date is null) throw new ArgumentNullException(nameof(UpdateSpecialRequest.Date));

        var updated = await _specialsRepository.UpdateAsync(id, request.Name, request.Date);
        return new UpdateSpecialResponse
        {
            Id = updated.Id,
            Name = updated.Name,
            Date = updated.Datetime
        };
    }
    
    public async Task<DeleteSpecialResponse> DeleteSpecialAsync(int id)
    {
        var deleted = await _specialsRepository.DeleteAsync(id);
        return new DeleteSpecialResponse
        {
            Id = deleted.Id,
            Date = deleted.Datetime,
            Name = deleted.Name
        };
    }

    public Task<List<Series>> GetSeriesAsync() => _seriesRepository.GetAllAsync();

    public Task<Series> GetSeriesDetailAsync(int id) => _seriesRepository.GetDetailAsync(id);

    public async Task<UpdateSeriesResponse> UpdateSeriesAsync(int id, UpdateSeriesRequest request)
    {
        var series = new Series
        {
            Id = id,
            Name = request.Name,
            Day = request.Day,
            Hours = request.Hours,
            Minutes = request.Minutes
        };
        var updated = await _seriesRepository.UpdateAsync(series);
        return new UpdateSeriesResponse
        {
            Id = updated.Id.Value,
            Name = updated.Name,
            Day = updated.Day,
            Hours = updated.Hours,
            Minutes = updated.Minutes
        };
    }

    public Task<List<Cancellation>> GetCancellationsAsync(int seriesId) =>
        _cancellationsRepository.GetAllAsync(seriesId);

    public Task<Cancellation> CreateCancellationAsync(int seriesId, DateOnly date, string? reason) =>
        _cancellationsRepository.CreateAsync(seriesId, date, reason);

    public async Task<UpdateCancellationResponse> UpdateCancellationAsync(int seriesId, int cancellationId, UpdateCancellationRequest request)
    {
        var updated = await _cancellationsRepository.UpdateAsync(seriesId,
            cancellationId, request.Date, request.Reason);
        return new UpdateCancellationResponse
        {
            Id = updated.Id,
            SeriesId = updated.SeriesId,
            Date = updated.Date,
            Reason = updated.Reason
        };
    }

    public async Task<DeleteCancellationResponse> DeleteCancellationAsync(int seriesId, int cancellationId)
    {
        var deleted = await _cancellationsRepository.DeleteAsync(seriesId, cancellationId);
        return new DeleteCancellationResponse
        {
            Id = deleted.Id,
            SeriesId = deleted.SeriesId,
            Date = deleted.Date,
            Reason = deleted.Reason
        };
    }
}
