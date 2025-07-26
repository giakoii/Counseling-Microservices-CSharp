using System.Net;
using System.Text.Json.Serialization;
using AppointmentService.Application.CounselorSchedules.Commands;
using AppointmentService.Application.CounselorSchedules.Consumers;
using AppointmentService.Application.CounselorSchedules.Queries;
using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;
using AppointmentService.Infrastructure.Data.Contexts;
using BuildingBlocks.Messaging.Events.CounselorScheduleEvents;
using Common.Utils.Const;
using DotNetEnv;
using JasperFx;
using Marten;
using MassTransit;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenIddict.Validation.AspNetCore;
using Shared.Infrastructure.Context;
using Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Logics;

var builder = WebApplication.CreateBuilder(args);

#region Environment

// Load environment variables from .env
Env.Load();

// Retrieve DB connection string
var connectionString = Environment.GetEnvironmentVariable(ConstEnv.AppointmentServiceDB);

#endregion

#region Database Configuration

// Register EF Core DbContext
builder.Services.AddDbContext<AppointmentServiceContext>(options =>
{ 
    options.UseNpgsql(connectionString);
});

// Register AppDbContext abstraction
builder.Services.AddScoped<AppDbContext, AppointmentServiceContext>();

#endregion

#region Marten Configuration (PostgreSQL Document Store)

// Configure Marten document database
builder.Services.AddMarten(options =>
{
    options.Connection(connectionString!);
    options.AutoCreateSchemaObjects = AutoCreate.All;
    options.DatabaseSchemaName = "AppointmentServiceDB_Marten";

    // Register all aggregate roots or document types
    options.Schema.For<WeekdayCollection>().Identity(x => x.Id);
    options.Schema.For<TimeSlotCollection>().Identity(x => x.Id);
    options.Schema.For<CounselorScheduleDetailCollection>().Identity(x => x.Id);
    options.Schema.For<PaymentCollection>().Identity(x => x.Id);
    options.Schema.For<AppointmentCollection>().Identity(x => x.Id);
    options.Schema.For<AdmissionDocumentCollection>().Identity(x => x.DocumentId);
});

#endregion

#region MediatR Configuration

// Register MediatR for application layer (CQRS)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(InsertCounselorScheduleCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(SelectCounselorSchedulesQuery).Assembly);
});

#endregion

#region MassTransit & RabbitMQ

// Configure messaging with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InsertCounselorScheduleEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    x.AddRequestClient<UserInformationRequest>();
});

#endregion

#region Registrations
builder.Services.AddHttpContextAccessor();

// Register application services
builder.Services.AddScoped<IIdentityService, IdentityService>();

// Generic repository registrations
builder.Services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
builder.Services.AddScoped(typeof(ISqlReadRepository<>), typeof(SqlReadRepository<>));
builder.Services.AddScoped(typeof(INoSqlQueryRepository<>), typeof(NoSqlRepository<>));

// Specific domain entity repositories (optional if needed separately)
builder.Services.AddScoped<ICommandRepository<CounselorScheduleDetail>, CommandRepository<CounselorScheduleDetail>>();
builder.Services.AddScoped<ICommandRepository<Weekday>, CommandRepository<Weekday>>();
builder.Services.AddScoped<ICommandRepository<TimeSlot>, CommandRepository<TimeSlot>>();

#endregion

#region Authentication & Authorization (OpenIddict)

// Configure OpenIddict authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

// Configure token validation
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer($"http://localhost:5050/");
        options.AddAudiences("service_client");

        options.UseIntrospection()
            .AddAudiences("service_client")
            .SetClientId("service_client")
            .SetClientSecret(Environment.GetEnvironmentVariable(ConstEnv.ClientSecret)!);

        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });

#endregion

#region Swagger / OpenAPI

// Configure OpenAPI (Swagger)
builder.Services.AddOpenApiDocument(config =>
{
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
    config.AddSecurity("JWT Token", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = nameof(Authorization),
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Copy this into the value field: Bearer {token}"
    });
});

#endregion

#region MVC / JSON Options

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

#endregion

#region CORS

// Allow cross-origin requests (CORS)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

#endregion

#region Data Protection

// Register Data Protection service
builder.Services.AddDataProtection();

#endregion

#region App Build & Middleware

var app = builder.Build();

// Development-only: expose Swagger UI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UsePathBase("/appointments");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();

app.Run();

#endregion