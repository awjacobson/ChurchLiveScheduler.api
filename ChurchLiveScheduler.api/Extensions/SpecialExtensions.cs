using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.sdk.Models;

namespace ChurchLiveScheduler.api.Extensions;

public static class SpecialExtensions
{
    public static SpecialDto ToDto(this Special entity) =>
        new()
        {
            Id = entity.Id,
            Date = entity.Datetime,
            Name = entity.Name
        };
}
