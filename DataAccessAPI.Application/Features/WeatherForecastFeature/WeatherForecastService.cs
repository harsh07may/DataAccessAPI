/* Plan:
 * Create a IApplicationDbContext within Application/Interfaces, register it Infrastructure/DependencyInjection.cs. 
 * With this we get rid of the unneccessary IRepository<T> abstraction 
 * & use EF Core directly within the Service layer.
 */
namespace DataAccessAPI.Application.Features.WeatherForecastFeature;
public class WeatherForecastService : IWeatherForecastService
{
    private readonly IAppDbContext _context;
    private readonly ILogger<WeatherForecastService> _logger;

    public WeatherForecastService(IAppDbContext context, ILogger<WeatherForecastService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<WeatherForecastDto>> GetAllForecastsAsync(CancellationToken cancellationToken = default)
    {
        var forecasts = await _context.WeatherForecasts
            .AsNoTracking()
            .Select(f => new WeatherForecastDto
            {
                Id = f.Id,
                Date = f.Date,
                TemperatureC = f.TemperatureC,
                TemperatureF = f.TemperatureF,
                Summary = f.Summary
            })
            .ToListAsync(cancellationToken);
        return forecasts;
    }

    public async Task<WeatherForecastDto?> GetForecastByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var forecast = await _context.WeatherForecasts
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new WeatherForecastDto
            {
                Id = f.Id,
                Date = f.Date,
                TemperatureC = f.TemperatureC,
                TemperatureF = f.TemperatureF,
                Summary = f.Summary
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (forecast == null)
            _logger.LogWarning("Weather forecast with ID: {Id} not found", id);

        return forecast;
    }

    public async Task<WeatherForecastDto> CreateForecastAsync(CreateWeatherForecastDto createDto, CancellationToken cancellationToken = default)
    {
        var entity = new WeatherForecast
        {
            Id = Guid.NewGuid(),
            Date = createDto.Date,
            TemperatureC = createDto.TemperatureC,
            Summary = createDto.Summary
        };

        await _context.WeatherForecasts.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created weather forecast with ID: {Id}", entity.Id);
        return new WeatherForecastDto
        {
            Id = entity.Id,
            Date = entity.Date,
            TemperatureC = entity.TemperatureC,
            TemperatureF = entity.TemperatureF,
            Summary = entity.Summary
        };
    }

    public async Task<WeatherForecastDto?> UpdateForecastAsync(Guid id, UpdateWeatherForecastDto updateDto, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WeatherForecasts.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
        {
            _logger.LogWarning("Weather forecast with ID: {Id} not found for update", id);
            return null;
        }

        entity.Date = updateDto.Date;
        entity.TemperatureC = updateDto.TemperatureC;
        entity.Summary = updateDto.Summary;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.WeatherForecasts.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated weather forecast with ID: {Id}", id);
        return new WeatherForecastDto
        {
            Id = entity.Id,
            Date = entity.Date,
            TemperatureC = entity.TemperatureC,
            TemperatureF = entity.TemperatureF,
            Summary = entity.Summary
        };
    }

    public async Task<bool> DeleteForecastAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WeatherForecasts.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
        {
            _logger.LogWarning("Weather forecast with ID: {Id} not found for deletion", id);
            return false;
        }

        _context.WeatherForecasts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted weather forecast with ID: {Id}", id);
        return true;
    }
}