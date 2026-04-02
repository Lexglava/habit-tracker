namespace PortfolioProject.Api.Contracts;

public sealed record HabitsStatsResponse(
    int TotalHabits,
    int CompletedLast7Days,
    double AvgCompletionRatePercent,
    int BestStreak);
