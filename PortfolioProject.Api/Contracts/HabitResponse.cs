namespace PortfolioProject.Api.Contracts;

public sealed record HabitResponse(
    Guid Id,
    string Name,
    string Category,
    int WeeklyTarget,
    int CurrentStreak,
    int TotalCompletions);
