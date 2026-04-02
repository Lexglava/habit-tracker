using PortfolioProject.Api.Contracts;
using PortfolioProject.Api.Models;

namespace PortfolioProject.Api.Services;

public sealed class HabitService : IHabitService
{
    private readonly List<HabitItem> _items =
    [
        new HabitItem { Name = "Read technical book", Category = "Learning", WeeklyTarget = 5 },
        new HabitItem { Name = "Solve algorithm task", Category = "Interview", WeeklyTarget = 4 }
    ];

    private readonly Lock _lock = new();

    public IReadOnlyCollection<HabitResponse> GetAll()
    {
        lock (_lock)
        {
            return _items
                .Select(ToResponse)
                .OrderByDescending(x => x.CurrentStreak)
                .ToArray();
        }
    }

    public HabitResponse Create(CreateHabitRequest request)
    {
        Validate(request);

        var item = new HabitItem
        {
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            WeeklyTarget = request.WeeklyTarget
        };

        lock (_lock)
        {
            _items.Add(item);
        }

        return ToResponse(item);
    }

    public HabitResponse? MarkComplete(Guid id, DateOnly completedOn)
    {
        lock (_lock)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item is null)
            {
                return null;
            }

            if (!item.CompletedDays.Contains(completedOn))
            {
                item.CompletedDays.Add(completedOn);
            }

            return ToResponse(item);
        }
    }

    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item is null)
            {
                return false;
            }

            _items.Remove(item);
            return true;
        }
    }

    public HabitsStatsResponse GetStats()
    {
        lock (_lock)
        {
            var since = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-6));
            var completedLast7Days = _items.Sum(x => x.CompletedDays.Count(d => d >= since));
            var avgRate = _items.Count == 0
                ? 0
                : _items.Average(x => Math.Min(100.0, x.CompletedDays.Count(d => d >= since) * 100.0 / x.WeeklyTarget));
            var bestStreak = _items.Count == 0 ? 0 : _items.Max(GetStreak);

            return new HabitsStatsResponse(_items.Count, completedLast7Days, Math.Round(avgRate, 2), bestStreak);
        }
    }

    public IReadOnlyCollection<DailyActivityPoint> GetWeeklyActivity()
    {
        lock (_lock)
        {
            var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-6));
            var points = new List<DailyActivityPoint>(capacity: 7);

            for (var i = 0; i < 7; i++)
            {
                var day = start.AddDays(i);
                var completed = _items.Count(item => item.CompletedDays.Contains(day));
                points.Add(new DailyActivityPoint(day.ToString("MM-dd"), completed));
            }

            return points;
        }
    }

    private static HabitResponse ToResponse(HabitItem item)
    {
        return new HabitResponse(
            item.Id,
            item.Name,
            item.Category,
            item.WeeklyTarget,
            GetStreak(item),
            item.CompletedDays.Count);
    }

    private static int GetStreak(HabitItem item)
    {
        if (item.CompletedDays.Count == 0)
        {
            return 0;
        }

        var sorted = item.CompletedDays.Distinct().OrderByDescending(x => x).ToList();
        var streak = 1;

        for (var i = 1; i < sorted.Count; i++)
        {
            if (sorted[i] == sorted[i - 1].AddDays(-1))
            {
                streak++;
            }
            else
            {
                break;
            }
        }

        return streak;
    }

    private static void Validate(CreateHabitRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Habit name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            throw new ArgumentException("Habit category is required.");
        }

        if (request.WeeklyTarget is < 1 or > 7)
        {
            throw new ArgumentException("Weekly target must be between 1 and 7.");
        }
    }
}
