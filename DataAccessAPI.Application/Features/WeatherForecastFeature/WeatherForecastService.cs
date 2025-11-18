using static System.Net.Mime.MediaTypeNames;
namespace DataAccessAPI.Application.Features.WeatherForecastFeature;

/* Plan:
 * Create a IApplicationDbContext within Application/Interfaces, register it Infrastructure/DependencyInjection.cs. 
 * With this we get rid of the unneccessary IRepository<T> abstraction 
 * & use EF Core directly within the Service layer.
 * 
 * TODO: Remove CancellationToken.None, Propogate CancellationToken: Controller -> Service: Frees up running db-queries when request is cancelled.
 */
public class WeatherForecastService : IWeatherForecastService
{

    private readonly IAppDbContext _context;

    public WeatherForecastService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WeatherForecastDto>> GetAllForecastsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WeatherForecasts
            .AsNoTracking()
            .Select(f => new WeatherForecastDto
            {
                Id = f.Id,
                Date = f.Date,
                TemperatureC = f.TemperatureC,
                TemperatureF = (f.TemperatureC * 9) / 5 + 32,
                Summary = f.Summary
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<WeatherForecastDto?> GetForecastByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WeatherForecasts
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new WeatherForecastDto
            {
                Id = f.Id,
                Date = f.Date,
                TemperatureC = f.TemperatureC,
                TemperatureF = (f.TemperatureC * 9) / 5 + 32,
                Summary = f.Summary
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WeatherForecastDto> CreateForecastAsync(CreateWeatherForecastDto createDto, CancellationToken cancellationToken = default)
    {
        var entity = new WeatherForecast
        {
            Date = createDto.Date,
            TemperatureC = createDto.TemperatureC,
            Summary = createDto.Summary
        };

        await _context.WeatherForecasts.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new WeatherForecastDto
        {
            Date = entity.Date,
            TemperatureC = entity.TemperatureC,
            TemperatureF = (entity.TemperatureC * 9) / 5 + 32,
            Summary = entity.Summary
        };
    }

    public async Task<WeatherForecastDto?> UpdateForecastAsync(Guid id, UpdateWeatherForecastDto updateDto, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WeatherForecasts.FindAsync(id, cancellationToken);
        if (entity is null)
            return null;

        entity.Date = updateDto.Date;
        entity.TemperatureC = updateDto.TemperatureC;
        entity.Summary = updateDto.Summary;

        _context.WeatherForecasts.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new WeatherForecastDto
        {
            Date = entity.Date,
            TemperatureC = entity.TemperatureC,
            TemperatureF = (entity.TemperatureC * 9) / 5 + 32,
            Summary = entity.Summary
        };
    }

    public async Task<bool> DeleteForecastAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WeatherForecasts.FindAsync(id, cancellationToken);
        if (entity is null)
            return false;

        _context.WeatherForecasts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}