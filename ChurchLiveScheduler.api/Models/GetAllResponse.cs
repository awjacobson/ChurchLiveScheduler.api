using System.Collections.Generic;

namespace ChurchLiveScheduler.api.Models;

public record GetAllResponse
{
    public IEnumerable<Series> Series { get; init; }
    public IEnumerable<Special> Specials { get; init;}
}
