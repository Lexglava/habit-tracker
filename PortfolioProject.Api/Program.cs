using PortfolioProject.Api.Contracts;
using PortfolioProject.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IHabitService, HabitService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGet("/", () => Results.Redirect("/index.html"));

var api = app.MapGroup("/api");
api.MapGet("/health", () => Results.Ok(new { status = "ok", service = "PortfolioProject.Api" }));

var habits = api.MapGroup("/habits");

habits.MapGet("/", (IHabitService service) => Results.Ok(service.GetAll()));

habits.MapPost("/", (CreateHabitRequest request, IHabitService service) =>
{
    try
    {
        var created = service.Create(request);
        return Results.Created($"/api/habits/{created.Id}", created);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

habits.MapPost("/{id:guid}/complete", (Guid id, CompleteHabitRequest request, IHabitService service) =>
{
    var completedOn = request.CompletedOn ?? DateOnly.FromDateTime(DateTime.UtcNow);
    var updated = service.MarkComplete(id, completedOn);

    return updated is null
        ? Results.NotFound(new { message = "Habit not found." })
        : Results.Ok(updated);
});

habits.MapDelete("/{id:guid}", (Guid id, IHabitService service) =>
{
    var removed = service.Delete(id);
    return removed ? Results.NoContent() : Results.NotFound(new { message = "Habit not found." });
});

habits.MapGet("/stats", (IHabitService service) => Results.Ok(service.GetStats()));
habits.MapGet("/activity/weekly", (IHabitService service) => Results.Ok(service.GetWeeklyActivity()));

app.Run();
