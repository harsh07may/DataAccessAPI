namespace DataAccessAPI.Domain.Entities;

public class WeatherForecast : BaseEntity<Guid>
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF =>  TempratureConstants.FREEZING_POINT_FAHRENHEIT + (int)(TemperatureC / TempratureConstants.CELSIUS_TO_FAHRENHEIT_RATIO);
    public string? Summary { get; set; }
}