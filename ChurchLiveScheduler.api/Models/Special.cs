namespace ChurchLiveScheduler.api.Models;

public record Special
{
    /// <summary>
    /// Gets the Id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the name
    /// </summary>
    /// <example>
    /// Easter Sunrise Service
    /// </example>
    public string Name { get; init; }

    /// <summary>
    /// Gets the date and time of the special
    /// </summary>
    /// <example>
    /// Easter Sunrise Service would be 2024-03-31T07:00:00
    /// </example>
    public string Datetime { get; init; }
}
