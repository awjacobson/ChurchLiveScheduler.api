using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ChurchLiveScheduler.api.Models;

[DebuggerDisplay("Date={Date}, Reason={Reason}")]
public record CreateCancellationRequest
{
    [JsonPropertyName("date")]
    public string Date { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}
