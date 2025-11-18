// Major TIL: Just appending a semi-colon at the end of the braces style namespace auto-converts it lol.

using DataAccessAPI.Application.Features.WeatherForecastFeature;
using DataAccessAPI.Domain.Entities;

namespace DataAccessAPI.Application.Interfaces;

public interface IWeatherForecastService
{
    Task<IEnumerable<WeatherForecastDto>> GetAllForecastsAsync(CancellationToken cancellationToken = default);
    Task<WeatherForecastDto?> GetForecastByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WeatherForecastDto> CreateForecastAsync(CreateWeatherForecastDto createDto, CancellationToken cancellationToken = default);
    Task<WeatherForecastDto?> UpdateForecastAsync(Guid id, UpdateWeatherForecastDto updateDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteForecastAsync(Guid id, CancellationToken cancellationToken = default);
}
