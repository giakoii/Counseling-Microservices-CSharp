var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseOpenApi();
app.UseSwaggerUi(settings =>
{
    settings.SwaggerRoutes.Add(new NSwag.AspNetCore.SwaggerUiRoute("Auth Service", "/auth/swagger/v1/swagger.json"));
    settings.SwaggerRoutes.Add(new NSwag.AspNetCore.SwaggerUiRoute("Appointment Service", "/appointments/swagger/v1/swagger.json"));
    settings.SwaggerRoutes.Add(new NSwag.AspNetCore.SwaggerUiRoute("Request Ticket Service", "/request-tickets/swagger/v1/swagger.json"));
    settings.SwaggerRoutes.Add(new NSwag.AspNetCore.SwaggerUiRoute("Chat Service", "/chat/swagger/v1/swagger.json"));
    settings.Path = "/swagger";
});
app.UseAuthorization();
app.MapReverseProxy();
app.Run();