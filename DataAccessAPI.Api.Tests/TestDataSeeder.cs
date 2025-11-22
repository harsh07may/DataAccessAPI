namespace DataAccessAPI.Api.Tests;

// Location: DataAccessAPI.Api.Tests/Helpers/TestDataSeeder.cs
public static class TestDataSeeder
{
    public static readonly Guid WarmId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ColdId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public const string WarmSummary = "Seeded Warm";
    public const string ColdSummary = "Seeded Freezing";

    public static async Task SeedWeatherForecastsAsync(AppDbContext context)
    {
        if (await context.WeatherForecasts.AnyAsync()) return;

        context.WeatherForecasts.AddRange(
            new WeatherForecast
            {
                Id = WarmId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                TemperatureC = 25,
                Summary = WarmSummary
            },
            new WeatherForecast
            {
                Id = ColdId,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                TemperatureC = -5,
                Summary = ColdSummary
            }
        );

        await context.SaveChangesAsync();
    }
}
