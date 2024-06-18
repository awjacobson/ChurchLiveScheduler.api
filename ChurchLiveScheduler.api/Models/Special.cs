using System.Diagnostics;

namespace ChurchLiveScheduler.api.Models;

[DebuggerDisplay("Name={Name}")]
public class Special
{
    /// <summary>
    /// Gets or sets the id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    /// <example>
    /// Easter Sunrise Service
    /// </example>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the special
    /// </summary>
    /// <example>
    /// Easter Sunrise Service would be 2024-03-31T07:00:00
    /// </example>
    public string Datetime { get; set; }
}
