using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AuthService.Infrastructure.Data.Contexts.QueryDbContext;

public class AuthServiceReadContext
{
    private readonly IMongoDatabase _database;

    public AuthServiceReadContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.Database);
    }
}

public class MongoDbSettings
{
    public string ConnectionString { get; set; }
    public string Database { get; set; }
}