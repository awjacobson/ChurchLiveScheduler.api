using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChurchLiveScheduler.api.Models;

[DebuggerDisplay("Name={Name}")]
public class Series
{
    /// <summary>
    /// Gets or sets the id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    /// <example>
    /// Sunday Morning Worship
    /// </example>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the day of week
    /// </summary>
    /// <example>
    /// DayOfWeek.Sunday
    /// </example>
    public DayOfWeek Day { get; set; }

    /// <summary>
    /// Gets or sets the start time hour
    /// </summary>
    /// <example>
    /// If the series was scheduled for 10:30 each week then Hours would be 10
    /// </example>
    public int Hours { get; set; }

    /// <summary>
    /// Gets or sets the start time minute
    /// </summary>
    /// <example>
    /// If the series was scheduled for 10:30 each week then Minutes would be 30
    /// </example>
    public int Minutes { get; set; }

    /// <summary>
    /// Gets or sets the cancellations for this series
    /// </summary>
    public IEnumerable<Cancellation> Cancellations { get; set; }
}
