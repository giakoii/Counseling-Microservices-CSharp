using System.Net;
using Common.Utils.Const;
using DotNetEnv;
using JasperFx;
using Marten;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenIddict.Validation.AspNetCore;
using RequestTicketService.Infrastructure.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);
// Load environment variables from .env file
Env.Load();

var postgreConnectionString = Environment.GetEnvironmentVariable(ConstEnv.RequestTicketServiceDB);

builder.Services.AddDataProtection();

builder.Services.AddDbContext<RequestTicketServiceContext>(options =>
{ 
    options.UseNpgsql(postgreConnectionString);
});

// Configure Marten for NoSQL operations
builder.Services.AddMarten(options =>
{
    options.Connection(postgreConnectionString);
    options.AutoCreateSchemaObjects = AutoCreate.All;
    options.DatabaseSchemaName = "RequestTicketServiceDB_Marten";
});

// Swagger configuration to output API type definitions
builder.Services.AddOpenApiDocument(config =>
{
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
    config.AddSecurity("JWT Token", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme()
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = nameof(Authorization),
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copy this into the value field: Bearer {token}"
        }
    );
});

// Allow API to be read from outside
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder => corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

builder.Services.AddOpenApi();

// Add Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure OpenIddict validation
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});


// Configure the OpenIddict server
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("https://localhost:5001/");
        options.AddAudiences("service_client");

        options.UseIntrospection()
            .AddAudiences("service_client")
            .SetClientId("service_client")
            .SetClientSecret(Environment.GetEnvironmentVariable(ConstEnv.ClientSecret)!);

        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.Run();