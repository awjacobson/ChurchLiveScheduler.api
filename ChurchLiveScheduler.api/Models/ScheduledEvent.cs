using System;
using System.Diagnostics;

namespace ChurchLiveScheduler.api.Models;

[DebuggerDisplay("Name={Name}, Start={Start}")]
public class ScheduledEvent
{
    public string Name { get; set; }
    public DateTime Start {  get; set; }
}
