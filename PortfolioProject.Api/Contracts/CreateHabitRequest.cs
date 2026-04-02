namespace PortfolioProject.Api.Contracts;

public sealed record CreateHabitRequest(
    string Name,
    string Category,
    int WeeklyTarget);
