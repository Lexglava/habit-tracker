using PortfolioProject.Api.Contracts;
using PortfolioProject.Api.Services;

namespace PortfolioProject.Tests;

public class HabitServiceTests
{
    [Fact]
    public void Create_ShouldAddHabit()
    {
        var service = new HabitService();

        var created = service.Create(new CreateHabitRequest("Practice C#", "Learning", 5));
        var all = service.GetAll();

        Assert.Contains(all, x => x.Id == created.Id && x.Name == "Practice C#");
    }

    [Fact]
    public void MarkComplete_ShouldIncreaseStreak_WhenDaysAreConsecutive()
    {
        var service = new HabitService();
        var created = service.Create(new CreateHabitRequest("Daily coding", "Career", 7));

        service.MarkComplete(created.Id, new DateOnly(2026, 3, 20));
        var afterSecond = service.MarkComplete(created.Id, new DateOnly(2026, 3, 21));

        Assert.NotNull(afterSecond);
        Assert.Equal(2, afterSecond.CurrentStreak);
    }

    [Fact]
    public void Create_ShouldThrow_WhenWeeklyTargetInvalid()
    {
        var service = new HabitService();

        Assert.Throws<ArgumentException>(() =>
            service.Create(new CreateHabitRequest("Test", "Career", 10)));
    }

    [Fact]
    public void GetWeeklyActivity_ShouldContainSevenPoints()
    {
        var service = new HabitService();

        var points = service.GetWeeklyActivity();

        Assert.Equal(7, points.Count);
    }
}
