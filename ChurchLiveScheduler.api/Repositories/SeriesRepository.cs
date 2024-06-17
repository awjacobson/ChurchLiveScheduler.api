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
    Task<List<Series>> GetAll();

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

    public async Task<List<Series>> GetAll()
    {
        return await _dbContext.Series
            .Include(x => x.Cancellations)
            .OrderBy(x => x.Day)
            .ThenBy(x => x.Hours)
            .ToListAsync();
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

    /// <summary>
    /// Get the date and time of the next occurance of the given series.
    /// </summary>
    /// <param name="now"></param>
    /// <param name="series"></param>
    /// <returns></returns>
    public static DateTime GetNextScheduledEvent(DateTime now, Series series) => GetNextDate(now, series.Day, series.Hours, series.Minutes);

    /// <summary>
    /// Get the date and time of the next day of the week and time (could be current date)
    /// </summary>
    /// <param name="now"></param>
    /// <param name="dayOfWeek"></param>
    /// <param name="hours"></param>
    /// <param name="minutes"></param>
    public static DateTime GetNextDate(DateTime now, DayOfWeek dayOfWeek, int hours, int minutes)
    {
        if (now.DayOfWeek == dayOfWeek && (hours >= now.Hour || (hours == now.Hour && minutes >= now.Minute)))
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
