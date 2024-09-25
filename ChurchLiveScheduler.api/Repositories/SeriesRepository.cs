using ChurchLiveScheduler.api.Extensions;
using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.sdk.Models;
using Microsoft.EntityFrameworkCore;

namespace ChurchLiveScheduler.api.Repositories;

internal interface ISeriesRepository
{
    /// <summary>
    /// Get all series
    /// </summary>
    /// <returns></returns>
    Task<List<SeriesDto>> GetAllAsync();

    /// <summary>
    /// Get a series
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<SeriesDto> GetDetailAsync(int id);

    /// <summary>
    /// Get the next in series after the given date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    Task<ScheduledEvent> GetNextAsync(DateTime date);

    Task<Series> UpdateAsync(Series series);
}

internal sealed class SeriesRepository(SchedulerDbContext dbContext) : ISeriesRepository
{
    public Task<List<SeriesDto>> GetAllAsync()
    {
        return dbContext.Series
            .Include(s => s.Cancellations)
            .OrderBy(s => s.Day)
            .ThenBy(s => s.Hours)
            .Select(s => s.ToDto())
            .ToListAsync();
    }

    public Task<SeriesDto> GetDetailAsync(int id)
    {
        return dbContext.Series
            .Where(s => s.Id == id)
            .Include(s => s.Cancellations)
            .Select(s => s.ToDto())
            .SingleAsync();
    }

    public async Task<ScheduledEvent> GetNextAsync(DateTime date)
    {
        var scheduledEvents = await dbContext.Series
            .Include(s => s.Cancellations)
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

    public Task<Series> UpdateAsync(Series series)
    {
        dbContext.Entry(series).State = EntityState.Modified;
        return dbContext.SaveChangesAsync().ContinueWith(_ => series);
    }
}
