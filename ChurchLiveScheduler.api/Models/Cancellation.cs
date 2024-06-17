using System;

namespace ChurchLiveScheduler.api.Models;

public record Cancellation
{
    /// <summary>
    /// Gets the Id of the series with a cancellation
    /// </summary>
    public int SeriesId { get; init; }

    /// <summary>
    /// Gets the date of the cancellation
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Gets the reason of the cancellation
    /// </summary>
    public string? Reason { get; init; }
}
