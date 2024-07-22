using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ChurchLiveScheduler.api.Models;

[DebuggerDisplay("Name={Name}")]
public class Series
{
    /// <summary>
    /// Gets or sets the id
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    /// <example>
    /// Sunday Morning Worship
    /// </example>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the day of week
    /// </summary>
    /// <example>
    /// DayOfWeek.Sunday
    /// </example>
    [JsonPropertyName("day")]
    public DayOfWeek Day { get; set; }

    /// <summary>
    /// Gets or sets the start time hour
    /// </summary>
    /// <example>
    /// If the series was scheduled for 10:30 each week then Hours would be 10
    /// </example>
    [JsonPropertyName("hours")]
    public int Hours { get; set; }

    /// <summary>
    /// Gets or sets the start time minute
    /// </summary>
    /// <example>
    /// If the series was scheduled for 10:30 each week then Minutes would be 30
    /// </example>
    [JsonPropertyName("minutes")]
    public int Minutes { get; set; }

    /// <summary>
    /// Gets or sets the cancellations for this series
    /// </summary>
    public IEnumerable<Cancellation> Cancellations { get; set; }
}
