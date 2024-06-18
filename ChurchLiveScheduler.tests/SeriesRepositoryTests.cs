using ChurchLiveScheduler.api.Repositories;

namespace ChurchLiveScheduler.tests;

[TestClass]
public sealed class SeriesRepositoryTests
{
    [TestMethod]
    public void GetNextDateOverloadOneShould_CheckCancellations()
    {
        // ARRANGE
        var now = DateTime.Parse("2024-03-24T10:33:00.000");
        var dayOfWeek = DayOfWeek.Sunday;
        var hours = 18;
        var minutes = 30;
        var cancellations = new List<DateOnly>() { DateOnly.Parse("2024-03-24") };

        // ACT
        var actual = SeriesRepository.GetNextDate(now, dayOfWeek, hours, minutes, cancellations);

        // ASSERT
        Assert.AreEqual(DateTime.Parse("2024-03-31T18:30:00.000"), actual);
    }

    [TestMethod]
    public void GetNextDateOverloadTwoShould()
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
