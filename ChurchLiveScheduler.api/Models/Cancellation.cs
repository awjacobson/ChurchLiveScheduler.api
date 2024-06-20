using System.Diagnostics;

namespace ChurchLiveScheduler.api.Models;

[DebuggerDisplay("Id={Id}, Reason={Reason}")]
public class Cancellation
{
    /// <summary>
    /// Gets or sets the id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Id of the series with a cancellation
    /// </summary>
    public int SeriesId { get; set; }

    /// <summary>
    /// Gets or sets the date of the cancellation
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the reason of the cancellation
    /// </summary>
    public string? Reason { get; set; }
}
