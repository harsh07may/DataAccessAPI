
using Microsoft.AspNetCore.Mvc;

namespace DataAccessAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _service;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
        IWeatherForecastService service,
        ILogger<WeatherForecastController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets all weather forecasts.
    /// </summary>
    /// <returns>List of weather forecasts</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeatherForecastDto>>> GetAll(CancellationToken token)
    {
        _logger.LogInformation("GET all weather forecasts requested");
        var forecasts = await _service.GetAllForecastsAsync(token);
        return Ok(forecasts);
    }

    /// <summary>
    /// Gets a specific weather forecast by ID.
    /// </summary>
    /// <param name="id">The forecast ID</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Weather forecast if found</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WeatherForecastDto>> GetById(Guid id, CancellationToken token)
    {
        _logger.LogInformation("GET weather forecast by ID: {Id}", id);

        var forecast = await _service.GetForecastByIdAsync(id, token);
        if (forecast == null)
            return NotFound(new { message = $"Forecast with ID {id} not found" });

        return Ok(forecast);
    }

    /// <summary>
    /// Creates a new weather forecast.
    /// </summary>
    /// <param name="forecast">Forecast data</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Created forecast</returns>
    [HttpPost]
    public async Task<ActionResult<WeatherForecastDto>> Create(
        [FromBody] CreateWeatherForecastDto forecast,
        CancellationToken token)
    {
        _logger.LogInformation("POST create weather forecast");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateForecastAsync(forecast, token);
        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    /// <summary>
    /// Updates an existing weather forecast.
    /// </summary>
    /// <param name="id">Forecast ID</param>
    /// <param name="forecast">Updated forecast data</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Updated forecast</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<WeatherForecastDto>> Update(
        Guid id,
        [FromBody] UpdateWeatherForecastDto forecast,
        CancellationToken token)
    {
        _logger.LogInformation("PUT update weather forecast ID: {Id}", id);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _service.UpdateForecastAsync(id, forecast, token);
        if (updated == null)
            return NotFound(new { message = $"Forecast with ID {id} not found" });

        return Ok(updated);
    }

    /// <summary>
    /// Deletes a weather forecast.
    /// </summary>
    /// <param name="id">Forecast ID</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        _logger.LogInformation("DELETE weather forecast ID: {Id}", id);

        var deleted = await _service.DeleteForecastAsync(id, token);
        if (!deleted)
            return NotFound(new { message = $"Forecast with ID {id} not found" });

        return NoContent();
    }
}