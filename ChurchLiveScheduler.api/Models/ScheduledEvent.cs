using System.Diagnostics;

namespace ChurchLiveScheduler.api.Models;

[DebuggerDisplay("Name={Name}, Start={Start}")]
public record ScheduledEvent
{
    /// <summary>
    /// Gets the name
    /// </summary>
    /// <remarks>
    /// This could be populated with the name of a series or special
    /// </remarks>
    public string Name { get; init; }

    /// <summary>
    /// Gets the start date and time
    /// </summary>
    /// <remarks>
    /// This could be populated with the start date and time of a series or special
    /// </remarks>
    public DateTime Start {  get; init; }
}
