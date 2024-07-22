using ChurchLiveScheduler.api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChurchLiveScheduler.api.Repositories;

internal interface ICancellationsRepository
{
    Task<List<Cancellation>> GetAllAsync(int seriesId);
    Task<Cancellation> FindAsync(int seriesId, int cancellationId);
    Task<Cancellation> CreateAsync(int seriesId, DateOnly date, string? reason);
    Task<Cancellation> UpdateAsync(int seriesId, int cancellationId, DateOnly date, string? reason);
    Task<Cancellation> DeleteAsync(int seriesId, int cancellationId);
}

internal sealed class CancellationsRepository : ICancellationsRepository
{
    private readonly SchedulerDbContext _dbContext;

    public CancellationsRepository(SchedulerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Cancellation>> GetAllAsync(int seriesId) =>
        _dbContext.Cancellations
            .Where(x => x.SeriesId == seriesId)
            .OrderByDescending(x => x.Date)
            .ToListAsync();

    public Task<Cancellation> FindAsync(int seriesId, int cancellationId) =>
        _dbContext.Cancellations.SingleAsync(x => x.Id == cancellationId && x.SeriesId == seriesId);

    public async Task<Cancellation> CreateAsync(int seriesId, DateOnly date, string? reason)
    {
        var entity = _dbContext.Cancellations.Add(new Cancellation
        {
            SeriesId = seriesId,
            Date = date,
            Reason = reason
        });
        await _dbContext.SaveChangesAsync();
        return entity.Entity;
    }


    public async Task<Cancellation> UpdateAsync(int seriesId, int cancellationId, DateOnly date, string? reason)
    {
        var existing = await FindAsync(seriesId, cancellationId);
        existing.SeriesId = seriesId;
        existing.Date = date;
        existing.Reason = reason;
        await _dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task<Cancellation> DeleteAsync(int seriesId, int cancellationId)
    {
        var existing = await FindAsync(seriesId, cancellationId);
        _dbContext.Cancellations.Remove(existing);
        await _dbContext.SaveChangesAsync();
        return existing;
    }
}
