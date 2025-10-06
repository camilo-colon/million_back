using million.domain.common;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace million.infrastructure.Common.Persistence.mongodb;

public class EntityConfiguration : IBsonMapConfiguration
{
    public void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Entity)))
        {
            BsonClassMap.RegisterClassMap<Entity>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.MapIdMember(c => c.Id)
                    .SetSerializer(GuidSerializer.StandardInstance);
            });
        }
    }
}
