using System.Net.Http.Json;

namespace DataAccessAPI.Api.Tests.Controllers;

public class WeatherForecastControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{

    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    private readonly Guid _seededId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _seededId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public WeatherForecastControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Runs EF Core migrations
        await context.Database.EnsureCreatedAsync();

        // Seed data
        await TestDataSeeder.SeedWeatherForecastsAsync(context);
    }

    public async Task DisposeAsync()
    {
        // Clean up data between tests if needed.
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GetAll_ShouldReturnSeededData()
    {
        // Act
        var response = await _client.GetAsync("/api/weatherforecast");
        
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<WeatherForecastDto>>();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Summary == "Seeded Warm");
        result.Should().Contain(x => x.Summary == "Seeded Freezing");
    }

}