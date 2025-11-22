/* Plan:
 * 
 *  Move top-level statements to proper methods
 *      1) CreateWebApplication(string[] args) 
 *      2) ConfigureAndRunApp(WebApplication app)
 *  Add SwaggerUI and point it to openapi and update launchsettings.json
 * 
 *  In .NET 10, The Program class is always public, so no need to declare it.
*/

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var app = CreateWebApplication(args);
await ConfigureAndRunApp(app);

WebApplication CreateWebApplication(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Health checks
    builder.Services.AddHealthChecks()
        .AddCheck<LivenessHealthCheck>("self")
        .AddCheck<ReadinessHealthCheck>("readiness");

    // Add services to the container.
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    builder.Services.AddControllers();
    builder.Services.AddProblemDetails();
    builder.Services.AddOpenApi();

    return builder.Build();
}
Task ConfigureAndRunApp(WebApplication app)
{
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("liveness"),
    });

    app.MapHealthChecks("/ready", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("readiness"),
    });

    return app.RunAsync();
}
