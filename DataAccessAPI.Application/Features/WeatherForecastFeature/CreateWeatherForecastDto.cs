namespace DataAccessAPI.Application.Features.WeatherForecastFeature;

public class CreateWeatherForecastDto
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF { get; set; }
    public string? Summary { get; set; }
}
