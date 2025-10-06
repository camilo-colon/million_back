using million.domain.common;

namespace million.domain.PropertyImages;

public class PropertyImage : Entity
{
    public Guid PropertyId { get; private init; }
    public string File { get; private init; }
    public bool  Enabled { get; private init; }

    public PropertyImage(Guid propertyId, string file, bool enabled, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        PropertyId = propertyId;
        File = file;
        Enabled = enabled;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public PropertyImage() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}