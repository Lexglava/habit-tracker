using PortfolioProject.Api.Contracts;

namespace PortfolioProject.Api.Services;

public interface IHabitService
{
    IReadOnlyCollection<HabitResponse> GetAll();
    HabitResponse Create(CreateHabitRequest request);
    HabitResponse? MarkComplete(Guid id, DateOnly completedOn);
    bool Delete(Guid id);
    HabitsStatsResponse GetStats();
    IReadOnlyCollection<DailyActivityPoint> GetWeeklyActivity();
}
