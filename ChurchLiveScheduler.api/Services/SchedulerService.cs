using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.api.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChurchLiveScheduler.api.Services;

public interface ISchedulerService
{
    Task<ScheduledEvent> GetScheduledEventAsync(DateTime date);
    Task<GetAllResponse> GetAllAsync();
}

/// <summary>
/// https://www.codeproject.com/Articles/5312004/Building-a-Micro-Web-API-with-Azure-Functions-and
/// </summary>
internal sealed class SchedulerService : ISchedulerService
{
    private readonly ISeriesRepository _seriesRepository;
    private readonly ISpecialsRepository _specialsRepository;

    public SchedulerService(ISeriesRepository seriesRepository,
        ISpecialsRepository specialsRepository)
    {
        _seriesRepository = seriesRepository;
        _specialsRepository = specialsRepository;
    }

    public async Task<ScheduledEvent> GetScheduledEventAsync(DateTime date)
    {
        var nextInSeriesTask = _seriesRepository.GetNextAsync(date);
        var nextInSpecialsTask = _specialsRepository.GetNextAsync(date);

        var nextInSeries = await nextInSeriesTask;
        var nextInSpecials = await nextInSpecialsTask;

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
            Series = await GetAllSeriesAsync(),
            Specials = await GetAllSpecialsAsync()
        };
    }

    public Task<List<Series>> GetAllSeriesAsync() => _seriesRepository.GetAll();

    public Task<List<Special>> GetAllSpecialsAsync() => _specialsRepository.GetAll();
}
