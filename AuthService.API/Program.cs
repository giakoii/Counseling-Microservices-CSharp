using System.Net;
using System.Text.Json.Serialization;
using AuthService.API;
using AuthService.API.Utils.Const;
using AuthService.Application.Services;
using AuthService.Application.Users.Commands;
using AuthService.Application.Users.Consumers;
using AuthService.Application.Users.Queries;
using AuthService.Domain.ReadModels;
using AuthService.Domain.WriteModels;
using AuthService.Infrastructure.Data.Contexts;
using AuthService.Infrastructure.Services;
using BuildingBlocks.Messaging.Events.CounselorScheduleEvents;
using DotNetEnv;
using JasperFx;
using Marten;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenIddict.Abstractions;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Context;
using Shared.Infrastructure.Logics;
using Shared.Infrastructure.Repositories;
using OpenApiSecurityScheme = NSwag.OpenApiSecurityScheme;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Get the connection string from environment variables
var connectionString = Environment.GetEnvironmentVariable(ConstEnv.AuthServiceDB);


builder.Services.AddDataProtection();
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AuthServiceContext>(options =>
{ 
    options.UseNpgsql(connectionString);
    options.UseOpenIddict();
});

// Configure Marten for NoSQL operations
builder.Services.AddMarten(options =>
{
    options.Connection(connectionString!);
    options.AutoCreateSchemaObjects = AutoCreate.All;
    options.DatabaseSchemaName = "AuthServiceDB_Marten";
    
    options.Schema.For<UserCollection>().Identity(x => x.Id);
});

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddScoped<AppDbContext, AuthServiceContext>();
builder.Services.AddScoped<IIdentityService, IdentityService>();


builder.Services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
builder.Services.AddScoped(typeof(ISqlReadRepository<>), typeof(SqlReadRepository<>));
builder.Services.AddScoped(typeof(INoSqlQueryRepository<>), typeof(NoSqlRepository<>));
builder.Services.AddScoped<ICommandRepository<User>, CommandRepository<User>>();
builder.Services.AddScoped<ICommandRepository<Role>, CommandRepository<Role>>();

builder.Services.AddScoped<ISendmailService, SendmailService>();
builder.Services.AddScoped<IUploadImageService, CloudinaryLogic>();
builder.Services.AddHttpContextAccessor();

// Add MassTransit with RabbitMQ
// Configure messaging with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SelectCounselorEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    x.AddRequestClient<SelectCounselorScheduleEvent>();
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(InsertUserCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(LoginUserCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(InsertCounselorCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(SelectCounselorQuery).Assembly);
});

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
        corsPolicyBuilder => corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

// Configure the OpenIdDict server
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<AppDbContext>();
    })
    .AddServer(options =>
    {
        options.DisableAccessTokenEncryption();
        options.AcceptAnonymousClients();
        // Enable the required endpoints
        options.SetTokenEndpointUris("/connect/token");
        options.SetIntrospectionEndpointUris("/connect/introspect");
        options.SetUserInfoEndpointUris("/connect/userinfo");
        options.SetEndSessionEndpointUris("/connect/logout");
        options.SetAuthorizationEndpointUris("/connect/authorize");
        options.AllowCustomFlow("google");
        // Enable the client credentials flow
        options.AllowPasswordFlow();
        options.AllowRefreshTokenFlow();
        options.AllowClientCredentialsFlow();
        options.AllowCustomFlow("logout");
        options.AllowCustomFlow("external");
        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
        options.RegisterScopes(OpenIddictConstants.Scopes.OfflineAccess);
        // Register the signing and encryption credentials
        options.UseReferenceAccessTokens();
        options.UseReferenceRefreshTokens();
        options.DisableAccessTokenEncryption();
        // Register your scopes
        options.RegisterScopes(
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles);
        // Register the encryption credentials
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();  
        
        // Set the lifetime of the tokens
        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(60));
        options.SetRefreshTokenLifetime(TimeSpan.FromMinutes(120));
        // Register ASP.NET Core host and configure options
        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableEndSessionEndpointPassthrough()
            .EnableAuthorizationEndpointPassthrough()
            .DisableTransportSecurityRequirement();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });


builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthServiceContext>();

    if (!context.Roles.Any())
    {
        context.Roles.AddRange(new[]
        {
            new Role { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" },
            new Role { Id = Guid.NewGuid(), Name = "Student", NormalizedName = "STUDENT"},
            new Role { Id = Guid.NewGuid(), Name = "Consultant", NormalizedName = "CONSULTANT" },
        });
        context.SaveChanges();
    }
}

app.UseCors();
app.UseDeveloperExceptionPage(); 
app.UseRouting();
app.UseAuthentication();
app.UseStatusCodePages(); 
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.Run();
