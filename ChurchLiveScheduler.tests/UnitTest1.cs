using ChurchLiveScheduler.api.Models;
using ChurchLiveScheduler.api.Repositories;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChurchLiveScheduler.tests;

[TestClass]
public class UnitTest1
{
    //private readonly ISeriesRepository _seriesRepository;
    //private readonly ISchedulerDbContext _schedulerDbContext;

    public UnitTest1()
    {
        //_seriesRepository = new SeriesRepository(_schedulerDbContext);
    }

    [TestMethod]
    public void GetNextDateShould()
    {
        // ARRANGE
        var now = DateTime.Parse("2024-03-24T10:33:00.000");
        var dayOfWeek = DayOfWeek.Sunday;
        var hours = 18;
        var minutes = 30;

        // ACT
        var actual = SeriesRepository.GetNextDate(now, dayOfWeek, hours, minutes);

        // ASSERT
        Assert.AreEqual(DateTime.Parse("2024-03-24T18:30:00.000"), actual);
    }

    [TestMethod]
    public void GetNextWeekdayDateShould()
    {
        // ARRANGE
        var now = DateTime.Parse("2024-03-24T10:33:00.000");
        var dayOfWeek = DayOfWeek.Sunday;

        // ACT
        var actual = SeriesRepository.GetNextWeekdayDate(now, dayOfWeek);

        // ASSERT
        Assert.AreEqual(DateTime.Parse("2024-03-31T10:33:00.000"), actual);
    }
}
