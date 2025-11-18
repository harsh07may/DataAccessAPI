
using Microsoft.AspNetCore.Mvc;

namespace DataAccessAPI.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecastDto>>> GetAll(CancellationToken token)
        {
            _logger.LogInformation("Fetching all weather forecasts");
            var forecasts = await _service.GetAllForecastsAsync(token);
            return Ok(forecasts);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WeatherForecastDto>> GetById(Guid id, CancellationToken token)
        {
            var forecast = await _service.GetForecastByIdAsync(id, token);
            if (forecast == null)
                return NotFound();

            return Ok(forecast);
        }

        [HttpPost]
        public async Task<ActionResult<WeatherForecastDto>> Create(CreateWeatherForecastDto forecast, CancellationToken token)
        {
            var created = await _service.CreateForecastAsync(forecast, token);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<WeatherForecastDto>> Update(Guid id, UpdateWeatherForecastDto forecast, CancellationToken token)
        {
            var updated = await _service.UpdateForecastAsync(id,forecast, token);
            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken token)
        {
            var deleted = await _service.DeleteForecastAsync(id, token);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

    }
}
