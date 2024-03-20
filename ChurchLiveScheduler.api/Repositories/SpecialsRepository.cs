using ChurchLiveScheduler.api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchLiveScheduler.api.Repositories;

public interface ISpecialsRepository
{
    Task<ScheduledEvent> GetNextAsync(DateTime date);
}

public sealed class SpecialsRepository : ISpecialsRepository
{
    private readonly SchedulerDbContext _dbContext;

    public SpecialsRepository(SchedulerDbContext dbContext)
    {
        _dbContext = dbContext;
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
