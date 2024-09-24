using ChurchLiveScheduler.api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChurchLiveScheduler.api.Repositories;

internal interface ISpecialsRepository
{
    /// <summary>
    /// Get all specials
    /// </summary>
    /// <returns></returns>
    Task<List<Special>> GetAllAsync();

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

internal sealed class SpecialsRepository : ISpecialsRepository
{
    private readonly SchedulerDbContext _dbContext;

    public SpecialsRepository(SchedulerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Special>> GetAllAsync()
    {
        return _dbContext.Specials
            .OrderByDescending(x => x.Datetime)
            .ToListAsync();
    }

    public Task<ScheduledEvent?> GetNextAsync(DateTime date)
    {
        var dateText = date.ToString("yyyy-MM-ddTHH:mm:00.000");

        return _dbContext.Specials
            .Where(x => string.Compare(x.Datetime, dateText) > 0)
            .OrderBy(x => x.Datetime)
            .Select(x => new ScheduledEvent { Name = x.Name, Start = DateTime.Parse(x.Datetime) })
            .FirstOrDefaultAsync();
    }

    public Task<Special> FindAsync(int id) => _dbContext.Specials.SingleAsync(x => x.Id == id);

    public async Task<Special> CreateAsync(string name, string date)
    {
        var entity = _dbContext.Specials.Add(new Special
        {
            Name = name,
            Datetime = date
        });
        await _dbContext.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task<Special> UpdateAsync(int id, string name, string date)
    {
        var existing = await FindAsync(id);
        existing.Name = name;
        existing.Datetime = date;
        await _dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task<Special> DeleteAsync(int id)
    {
        var existing = await FindAsync(id);
        _dbContext.Specials.Remove(existing);
        await _dbContext.SaveChangesAsync();
        return existing;
    }
}
