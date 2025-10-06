using million.domain.properties;
using million.infrastructure.Common.Persistence;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace million.infrastructure.Properties.Persistence;

public class PropertyConfiguration : IBsonMapConfiguration
{
    public void Configure()
    {
        BsonClassMap.RegisterClassMap<Property>(cm =>
        {
            cm.AutoMap();
            
            cm.MapProperty(c => c.CodeInternal).SetElementName("code_internal");
            cm.MapProperty(c => c.Address).SetElementName("address");
            cm.MapProperty(c => c.Name).SetElementName("name");
            cm.MapProperty(c => c.Price).SetElementName("price");
            cm.MapProperty(c => c.Year).SetElementName("year");
            
            cm.MapProperty(c => c.OwnerId).SetElementName("owner_id").SetSerializer(GuidSerializer.StandardInstance);
            
            cm.SetIgnoreExtraElements(true);
        });
    }
}
