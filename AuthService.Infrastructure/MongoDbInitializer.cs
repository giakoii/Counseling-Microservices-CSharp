using AuthService.Domain.ReadModels;
using MongoDB.Driver;

namespace AuthService.Infrastructure;

public class MongoDbInitializer
{
    private readonly IMongoDatabase _database;

    public MongoDbInitializer(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
    }

    public void InitializeCollections()
    {
        var existingCollections = _database.ListCollectionNames().ToList();

        if (!existingCollections.Contains(nameof(UserMongo)))
            _database.CreateCollection("Users");

        if (!existingCollections.Contains(nameof(RoleMongo)))
            _database.CreateCollection("Roles");
    }
}