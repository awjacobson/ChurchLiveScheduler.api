using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.sdk.Models;

namespace ChurchLiveScheduler.api.Extensions;

public static class CancellationExtensions
{
    public static CancellationDto ToDto(this Cancellation entity) =>
        new()
        {
            Id = entity.Id,
            SeriesId = entity.SeriesId,
            Date = entity.Date,
            Reason = entity.Reason
        };
}
