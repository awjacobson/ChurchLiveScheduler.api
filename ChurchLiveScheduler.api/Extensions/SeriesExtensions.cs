using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.sdk.Models;

namespace ChurchLiveScheduler.api.Extensions;

public static class SeriesExtensions
{
    public static SeriesDto ToDto(this Series entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Day = entity.Day,
            Hours = entity.Hours,
            Minutes = entity.Minutes,
            Cancellations = entity.Cancellations?.Select(c => c.ToDto()),
        };

    public static UpdateSeriesResponse ToUpdateResponse(this Series entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Day = entity.Day,
            Hours = entity.Hours,
            Minutes = entity.Minutes
        };
}
