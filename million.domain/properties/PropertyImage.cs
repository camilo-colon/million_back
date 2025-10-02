using million.domain.common;

namespace million.domain.properties;

public class PropertyImage : Entity
{
    public string File { get; }
    public bool  Enabled { get; }

    public PropertyImage(string file, bool enabled, Guid? id) : base(id ?? Guid.NewGuid())
    {
        File = file;
        Enabled = enabled;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private PropertyImage() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}