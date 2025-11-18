/* Plan:
 - Move top-level statements to proper methods
 -      1) CreateWebApplication(string[] args) 
 -      2) ConfigureAndRunApp(WebApplication app)
 -  Add SwaggerUI and point it to openapi and update launchsettings.json
*/

var app = CreateWebApplication(args);
await ConfigureAndRunApp(app);

WebApplication CreateWebApplication(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    return builder.Build();
}
Task ConfigureAndRunApp(WebApplication app)
{

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

    return app.RunAsync();
}
