using ChurchLiveScheduler.api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchLiveScheduler.api.Repositories;

internal interface ISeriesRepository
{
    /// <summary>
    /// Get all series
    /// </summary>
    /// <returns></returns>
    Task<List<Series>> GetAllAsync();

    Task<Series> GetDetailAsync(int seriesId);

    /// <summary>
    /// Get the next in series after the given date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    Task<ScheduledEvent> GetNextAsync(DateTime date);
}

internal sealed class SeriesRepository : ISeriesRepository
{
    private readonly SchedulerDbContext _dbContext;

    public SeriesRepository(SchedulerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Series>> GetAllAsync()
    {
        return _dbContext.Series
            .Include(x => x.Cancellations)
            .OrderBy(x => x.Day)
            .ThenBy(x => x.Hours)
            .ToListAsync();
    }

    public Task<Series> GetDetailAsync(int seriesId)
    {
        return _dbContext.Series
            .Where(x => x.Id == seriesId)
            .Include(x => x.Cancellations)
            .SingleAsync();
    }

    public async Task<ScheduledEvent> GetNextAsync(DateTime date)
    {
        var scheduledEvents = await _dbContext.Series
            .Select(x => new ScheduledEvent
            {
                Name = x.Name,
                Start = GetNextScheduledEvent(date, x)
            })
            .ToListAsync();

        return scheduledEvents.OrderBy(x => x.Start).FirstOrDefault();
    }

    public static DateTime GetNextScheduledEvent(DateTime now, Series series) =>
        GetNextDate(now, series.Day, series.Hours, series.Minutes, series.Cancellations?.Select(x => x.Date));

    /// <summary>
    /// Get the next date that is not cancelled
    /// </summary>
    /// <param name="now"></param>
    /// <param name="dayOfWeek"></param>
    /// <param name="hours"></param>
    /// <param name="minutes"></param>
    /// <param name="cancellations"></param>
    /// <returns></returns>
    public static DateTime GetNextDate(DateTime now, DayOfWeek dayOfWeek, int hours, int minutes, IEnumerable<DateOnly>? cancellations)
    {
        do
        {
            var next = GetNextDate(now, dayOfWeek, hours, minutes);
            if (cancellations == null || !cancellations.Contains(DateOnly.FromDateTime(next)))
            {
                return next;
            }
            now = next.AddMinutes(1);
        }
        while (true);
    }

    /// <summary>
    /// Get the date and time of the next day of the week and time (could be current date)
    /// </summary>
    /// <param name="now"></param>
    /// <param name="dayOfWeek"></param>
    /// <param name="hours"></param>
    /// <param name="minutes"></param>
    public static DateTime GetNextDate(DateTime now, DayOfWeek dayOfWeek, int hours, int minutes)
    {
        if (now.DayOfWeek == dayOfWeek && (hours > now.Hour || (hours == now.Hour && minutes >= now.Minute)))
        {
            return new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);
        }

        var nextDate = GetNextWeekdayDate(now, dayOfWeek);
        return new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, hours, minutes, 0);
    }

    /// <summary>
    /// Get the date of the next day in week
    /// </summary>
    /// <param name="now"></param>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static DateTime GetNextWeekdayDate(DateTime now, DayOfWeek dayOfWeek)
    {
        var daysUntilDayOfWeek = ((int)dayOfWeek - 1 - (int)now.DayOfWeek + 7) % 7 + 1;
        return now.AddDays(daysUntilDayOfWeek);
    }
}
