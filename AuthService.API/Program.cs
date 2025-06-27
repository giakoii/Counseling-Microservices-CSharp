using System.Net;
using AuthService.API;
using AuthService.API.Utils.Const;
using AuthService.Application.Users.Commands;
using AuthService.Domain.WriteModels;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Data.Contexts.CommandDbContext;
using Common.SystemClient;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenIddict.Abstractions;
using Shared.Application.Repositories;
using Shared.Infrastructure.MongoDB.Repositories;
using Shared.Infrastructure.PostgreSQL.Context;
using Shared.Infrastructure.PostgreSQL.Repositories;
using OpenApiSecurityScheme = NSwag.OpenApiSecurityScheme;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Get the connection string from environment variables
var postgreConnectionString = Environment.GetEnvironmentVariable(ConstEnv.ConnectionString);
var mongoConnectionString = Environment.GetEnvironmentVariable(ConstEnv.MonGoConnectionString);


builder.Services.AddDataProtection();
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AuthServiceContext>(options =>
{ 
    options.UseNpgsql(postgreConnectionString);
    options.UseOpenIddict();
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = MongoClientSettings.FromConnectionString(mongoConnectionString);
    return new MongoClient(settings);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("AuthServiceDB");
});


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<AppDbContext, AuthServiceContext>();
builder.Services.AddScoped<IIdentityApiClient, IdentityApiClient>();


builder.Services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
builder.Services.AddScoped(typeof(ISqlQueryRepository<>), typeof(SqlQueryRepository<>));
builder.Services.AddScoped(typeof(INoSqlQueryRepository<>), typeof(NoSqlQueryRepository<>));
builder.Services.AddScoped<ICommandRepository<User>, CommandRepository<User>>();
builder.Services.AddScoped<ICommandRepository<Role>, CommandRepository<Role>>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(InsertUserCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(LoginUserCommand).Assembly);
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
    // config.DocumentProcessors.Add(new OrderOperationsProcessor);
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
            new Role { RoleId = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" },
            new Role { RoleId = Guid.NewGuid(), Name = "Customer", NormalizedName = "CUSTOMER"},
            new Role { RoleId = Guid.NewGuid(), Name = "Consultant", NormalizedName = "CONSULTANT" },
        });
        context.SaveChanges();
    }
}

using (var scope = app.Services.CreateScope())
{
    var mongoClientDB = scope.ServiceProvider.GetRequiredService<IMongoClient>();
    
    var initializer = new MongoDbInitializer(mongoClientDB, "AuthServiceDB");
    initializer.InitializeCollections();
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

