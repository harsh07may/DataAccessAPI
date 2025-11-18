using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessAPI.Infrastructure.Persistence.Configurations;

public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecast>
{
    public void Configure(EntityTypeBuilder<WeatherForecast> builder)
    {
        builder.HasKey(e => e.Id);

        // Configure DB-side default for GUIDs on SQL Server:
        // This creates a DEFAULT constraint: NEWSEQUENTIALID()
        // EF Core will populate the Id after SaveChangesAsync().
        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()");

    }
}