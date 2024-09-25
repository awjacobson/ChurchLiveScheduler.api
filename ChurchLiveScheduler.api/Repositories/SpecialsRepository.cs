using ChurchLiveScheduler.api.Extensions;
using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.sdk.Models;
using Microsoft.EntityFrameworkCore;

namespace ChurchLiveScheduler.api.Repositories;

internal interface ISpecialsRepository
{
    /// <summary>
    /// Get all specials
    /// </summary>
    /// <returns></returns>
    Task<List<SpecialDto>> GetAllAsync();

    /// <summary>
    /// Get the next special after the given date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    Task<ScheduledEvent?> GetNextAsync(DateTime date);

    Task<Special> FindAsync(int id);

    Task<Special> CreateAsync(string name, string date);

    Task<Special> UpdateAsync(int id, string name, string date);

    Task<Special> DeleteAsync(int id);
}

internal sealed class SpecialsRepository(SchedulerDbContext dbContext) : ISpecialsRepository
{
    public Task<List<SpecialDto>> GetAllAsync()
    {
        return dbContext.Specials
            .OrderByDescending(x => x.Datetime)
            .Select(s => s.ToDto())
            .ToListAsync();
    }

    public Task<ScheduledEvent?> GetNextAsync(DateTime date)
    {
        var dateText = date.ToString("yyyy-MM-ddTHH:mm:00.000");

        return dbContext.Specials
            .Where(x => string.Compare(x.Datetime, dateText) > 0)
            .OrderBy(x => x.Datetime)
            .Select(x => new ScheduledEvent { Name = x.Name, Start = DateTime.Parse(x.Datetime) })
            .FirstOrDefaultAsync();
    }

    public Task<Special> FindAsync(int id) => dbContext.Specials.SingleAsync(x => x.Id == id);

    public async Task<Special> CreateAsync(string name, string date)
    {
        var entity = dbContext.Specials.Add(new Special
        {
            Name = name,
            Datetime = date
        });
        await dbContext.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task<Special> UpdateAsync(int id, string name, string date)
    {
        var existing = await FindAsync(id);
        existing.Name = name;
        existing.Datetime = date;
        await dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task<Special> DeleteAsync(int id)
    {
        var existing = await FindAsync(id);
        dbContext.Specials.Remove(existing);
        await dbContext.SaveChangesAsync();
        return existing;
    }
}
