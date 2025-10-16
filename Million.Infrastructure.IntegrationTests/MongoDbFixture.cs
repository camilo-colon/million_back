using million.infrastructure.Common.Persistence;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Million.Infrastructure.IntegrationTests;

[SetUpFixture]
public class MongoDbFixture
{
    private static MongoDbContainer? _mongoContainer;
    private static IMongoDatabase? _database;

    public static IMongoDatabase Database => _database
        ?? throw new InvalidOperationException("Database not initialized. Run OneTimeSetUp first.");

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        BsonClassMapRegister.RegisterFromAssembly(typeof(BsonClassMapRegister).Assembly);
        
        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .WithUsername("admin")
            .WithPassword("password")
            .Build();

        await _mongoContainer.StartAsync();

        var client = new MongoClient(_mongoContainer.GetConnectionString());
        _database = client.GetDatabase("million_test");
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_mongoContainer != null)
        {
            await _mongoContainer.StopAsync();
            await _mongoContainer.DisposeAsync();
        }
    }

    public static async Task CleanDatabase()
    {
        if (_database == null) return;

        var collections = await _database.ListCollectionNames().ToListAsync();
        foreach (var collection in collections)
        {
            await _database.DropCollectionAsync(collection);
        }
    }
}
