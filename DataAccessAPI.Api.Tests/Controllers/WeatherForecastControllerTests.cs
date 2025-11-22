namespace DataAccessAPI.Api.Tests.Controllers;

public class WeatherForecastControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{

    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public WeatherForecastControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();  
        
        // Run Migrations & Seed database.
        await context.Database.EnsureCreatedAsync();
        if (await context.WeatherForecasts.AnyAsync()) return;

        context.WeatherForecasts.AddRange(
            new WeatherForecast
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Date = DateOnly.FromDateTime(DateTime.Now),
                TemperatureC = 25,
                Summary = "Seeded Warm"
            },
            new WeatherForecast
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                TemperatureC = -5,
                Summary = "Seeded Freezing"
            }
        );
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        // Clean up data between tests.
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureDeletedAsync();
    }

    #region GetAll
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
    #endregion
   
    #region GetById
    [Fact]
    public async Task GetById_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var existingId = "00000000-0000-0000-0000-000000000001";

        // Act
        var response = await _client.GetAsync($"/api/weatherforecast/{existingId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<WeatherForecastDto>();
        result.Should().NotBeNull();
        result.Id.Should().Be(existingId);

    }

    [Fact]
    public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var existingId = "11111111-1111-1111-1111-111111111111";

        // Act
        var response = await _client.GetAsync($"/api/weatherforecast/{existingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    #endregion

    #region Create
    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var newForecast = new CreateWeatherForecastDto
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            TemperatureC = 25,
            Summary = "Integration Test Sunny"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/weatherforecast", newForecast);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<WeatherForecastDto>();
        result.Should().NotBeNull();
        result!.Summary.Should().Be(newForecast.Summary);
        result.Id.Should().NotBeEmpty();

    }
    #endregion

    #region Update
    [Fact]
    public async Task Update_WithExistingId_ShouldReturnUpdatedForecast()
    {
        // Arrange
        var existingId = "00000000-0000-0000-0000-000000000001";

        var updateDto = new UpdateWeatherForecastDto
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            TemperatureC = 30,
            Summary = "Updated Heatwave"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/weatherforecast/{existingId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<WeatherForecastDto>();
        result!.Summary.Should().Be("Updated Heatwave");
        result.TemperatureC.Should().Be(30);

    }
    #endregion

    #region Delete
    [Fact]
    public async Task Delete_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange
        var existingId = "00000000-0000-0000-0000-000000000001";

        // Act
        var response = await _client.DeleteAsync($"/api/weatherforecast/{existingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it is actually gone
        var getResponse = await _client.GetAsync($"/api/weatherforecast/{existingId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/weatherforecast/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    #endregion
}