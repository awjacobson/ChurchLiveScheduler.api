using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.sdk.Models;

namespace ChurchLiveScheduler.api.Services;

public interface ISchedulerService
{
    Task<ScheduledEvent> GetScheduledEventAsync(DateTime date);
    Task<GetAllResponse> GetAllAsync();

    Task<List<SpecialDto>> GetSpecialsAsync();
    Task<CreateSpecialResponse> CreateSpecial(CreateSpecialRequest request);
    Task<UpdateSpecialResponse> UpdateSpecialAsync(int id, UpdateSpecialRequest request);
    Task<DeleteSpecialResponse> DeleteSpecialAsync(int id);

    Task<List<SeriesDto>> GetSeriesAsync();
    Task<SeriesDto> GetSeriesDetailAsync(int id);
    Task<UpdateSeriesResponse> UpdateSeriesAsync(int id, UpdateSeriesRequest request);

    Task<List<Cancellation>> GetCancellationsAsync(int seriesId);
    Task<Cancellation> CreateCancellationAsync(int seriesId, DateOnly date, string? reason);
    Task<UpdateCancellationResponse> UpdateCancellationAsync(int seriesId, int cancellationId, UpdateCancellationRequest request);
    Task<DeleteCancellationResponse> DeleteCancellationAsync(int seriesId, int cancellationId);
}
