using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.api.Repositories;

namespace ChurchLiveScheduler.api.Services;

public interface ISchedulerService
{
    Task<ScheduledEvent> GetScheduledEventAsync(DateTime date);
    Task<GetAllResponse> GetAllAsync();
    Task<List<Special>> GetSpecialsAsync();
    Task<List<Series>> GetSeriesAsync();
    Task<Series> GetSeriesDetailAsync(int seriesId);
    Task<List<Cancellation>> GetCancellationsAsync(int seriesId);
    Task<Cancellation> CreateCancellationAsync(int seriesId, DateOnly date, string? reason);
    Task<Cancellation> UpdateCancellationAsync(int seriesId, int cancellationId, DateOnly date, string? reason);
    Task<Cancellation> DeleteCancellationAsync(int seriesId, int cancellationId);
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
        return new GetAllResponse
        {
            Series = await GetSeriesAsync(),
            Specials = await GetSpecialsAsync()
        };
    }

    public Task<List<Special>> GetSpecialsAsync() => _specialsRepository.GetAllAsync();

    public Task<List<Series>> GetSeriesAsync() => _seriesRepository.GetAllAsync();

    public Task<Series> GetSeriesDetailAsync(int seriesId) => _seriesRepository.GetDetailAsync(seriesId);

    public Task<List<Cancellation>> GetCancellationsAsync(int seriesId) =>
        _cancellationsRepository.GetAllAsync(seriesId);

    public Task<Cancellation> CreateCancellationAsync(int seriesId, DateOnly date, string? reason) =>
        _cancellationsRepository.CreateAsync(seriesId, date, reason);

    public Task<Cancellation> UpdateCancellationAsync(int seriesId, int cancellationId, DateOnly date, string? reason) =>
        _cancellationsRepository.UpdateAsync(seriesId, cancellationId, date, reason);

    public Task<Cancellation> DeleteCancellationAsync(int seriesId, int cancellationId) =>
        _cancellationsRepository.DeleteAsync(seriesId, cancellationId);
}
