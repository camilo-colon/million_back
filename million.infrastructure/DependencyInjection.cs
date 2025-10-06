using million.domain.PropertyImages;
using million.infrastructure.Common.Persistence;
using million.infrastructure.Properties.Persistence;
using million.infrastructure.PropertyImages;
using million.infrastructure.PropertyImages.Persistence;

namespace million.infrastructure;

using domain.properties;
using MongoDB.Driver;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        BsonClassMapRegister.RegisterFromAssembly(typeof(BsonClassMapRegister).Assembly);
        
        var connectionString = configuration.GetSection("MongoDbSettings:ConnectionString").Value;
        var databaseName = configuration.GetSection("MongoDbSettings:DatabaseName").Value;
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        services.AddSingleton(database);
        
        services.AddScoped<IPropertyRepository, PropertyMongoRepository>();
        services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
        
        return services;
    }
}