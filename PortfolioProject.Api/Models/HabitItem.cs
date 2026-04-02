namespace PortfolioProject.Api.Models;

public sealed class HabitItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public required string Category { get; init; }
    public int WeeklyTarget { get; init; }
    public DateOnly CreatedOn { get; init; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public List<DateOnly> CompletedDays { get; } = new();
}
