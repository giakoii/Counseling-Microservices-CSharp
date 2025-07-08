using System.Net;
using System.Text.Json.Serialization;
using AppointmentService.Application.CounselorSchedules.Commands;
using AppointmentService.Application.CounselorSchedules.Consumers;
using AppointmentService.Domain;
using AppointmentService.Infrastructure.Data.Contexts;
using BuildingBlocks.Messaging.Events.InsertCounselorSchedule;
using Common.Utils.Const;
using DotNetEnv;
using JasperFx;
using Marten;
using MassTransit;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenIddict.Validation.AspNetCore;
using Shared.Application.Repositories;
using Shared.Infrastructure.Context;
using Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

Env.Load();

// Get the connection string from environment variables
var connectionString = Environment.GetEnvironmentVariable(ConstEnv.AppointmentServiceDB);

builder.Services.AddDataProtection();

builder.Services.AddDbContext<AppointmentServiceContext>(options =>
{ 
    options.UseNpgsql(connectionString);
});

// Configure Marten for NoSQL operations
builder.Services.AddMarten(options =>
{
    options.Connection(connectionString);
    options.AutoCreateSchemaObjects = AutoCreate.All;
    options.DatabaseSchemaName = "AppointmentServiceDB_Marten";
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(InsertCounselorScheduleCommand).Assembly);
});

// // Register the DbContext for SQL operations
builder.Services.AddScoped<AppDbContext, AppointmentServiceContext>();

// Add MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InsertCounselorScheduleEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    x.AddRequestClient<InsertCounselorScheduleRequest>();
});

builder.Services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
builder.Services.AddScoped(typeof(ISqlReadRepository<>), typeof(SqlReadRepository<>));
builder.Services.AddScoped(typeof(INoSqlQueryRepository<>), typeof(NoSqlRepository<>));
builder.Services.AddScoped<ICommandRepository<CounselorSchedule> , CommandRepository<CounselorSchedule>>();
builder.Services.AddScoped<ICommandRepository<Weekday> , CommandRepository<Weekday>>();
builder.Services.AddScoped<ICommandRepository<TimeSlot> , CommandRepository<TimeSlot>>();

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddOpenApi();

// Swagger configuration to output API type definitions
builder.Services.AddOpenApiDocument(config =>
{
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
    config.AddSecurity("JWT Token", Enumerable.Empty<string>(),
        new OpenApiSecurityScheme()
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
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

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