using System;

namespace ChurchLiveScheduler.api.Models;

public class Series
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DayOfWeek Day { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
}
