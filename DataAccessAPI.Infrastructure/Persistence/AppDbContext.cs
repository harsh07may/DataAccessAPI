using DataAccessAPI.Application.Interfaces;

namespace DataAccessAPI.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options,
        ICurrentUserService currentUserService) : base(options) 
    {
        _currentUserService = currentUserService;
    }

    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;
        var timestamp = DateTime.UtcNow;

        // 1. Detect Changes & snapshot the entries immediately
        var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
                .ToList();

        // 2. Create Audit Records
        foreach (var entry in entries)
        {
            // NOTE: For a simple audit, We accept that ID on Insert might be missed, 
            // We may rely on EF Core client-side GUID generation if enabled.
            // This breaks when DB-side NEWSEQUENTIALID()
            var entityId = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue?.ToString();

            AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = entry.State.ToString(),
                EntityName = entry.Entity.GetType().Name,
                EntityId = entityId,
                Timestamp = timestamp
            });
        }

        // 3. Save everything (Original changes + Audit logs) in one transaction
        return await base.SaveChangesAsync(cancellationToken);
    }
}
