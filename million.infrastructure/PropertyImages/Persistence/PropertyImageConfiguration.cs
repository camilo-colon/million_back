using million.domain.PropertyImages;
using million.infrastructure.Common.Persistence;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace million.infrastructure.PropertyImages.Persistence;

public class PropertyImageConfiguration : IBsonMapConfiguration
{
    public void Configure()
    {
        BsonClassMap.RegisterClassMap<PropertyImage>(cm =>
        {
            cm.AutoMap();

            cm.MapProperty(c => c.Enabled)
                .SetElementName("enabled");

            cm.MapProperty(c => c.File)
                .SetElementName("file");
            
            cm.MapProperty(p => p.PropertyId)
                .SetElementName("property_id")
                .SetSerializer(new GuidSerializer(BsonType.String));
            
            cm.SetIgnoreExtraElements(true);
        });
    }
}