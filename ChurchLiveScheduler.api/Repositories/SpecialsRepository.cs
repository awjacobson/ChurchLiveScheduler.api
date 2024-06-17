using ChurchLiveScheduler.api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchLiveScheduler.api.Repositories;

internal interface ISpecialsRepository
{
    /// <summary>
    /// Get all specials
    /// </summary>
    /// <returns></returns>
    Task<List<Special>> GetAll();

    /// <summary>
    /// Get the next special after the given date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    Task<ScheduledEvent> GetNextAsync(DateTime date);
}

internal sealed class SpecialsRepository : ISpecialsRepository
{
    private readonly SchedulerDbContext _dbContext;

    public SpecialsRepository(SchedulerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Special>> GetAll()
    {
        return _dbContext.Specials
            .OrderByDescending(x => x.Datetime)
            .ToListAsync();
    }

    public Task<ScheduledEvent> GetNextAsync(DateTime date)
    {
        var dateText = date.ToString("yyyy-MM-ddTHH:mm:00.000");

        return _dbContext.Specials
            .Where(x => String.Compare(x.Datetime, dateText) > 0)
            .OrderBy(x => x.Datetime)
            .Select(x => new ScheduledEvent { Name = x.Name, Start = DateTime.Parse(x.Datetime) })
            .FirstOrDefaultAsync();
    }
}
