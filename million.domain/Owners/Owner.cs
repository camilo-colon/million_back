using million.domain.common;

namespace million.domain.owners;

public class Owner : Entity
{
    public readonly List<string> _propertyIds = [];
    public string Name { get; }
    public string Address { get; }
    public string Photo { get; } 
    public DateTime Birthday { get; }

    public Owner(string name, string address, string photo, DateTime birthday, Guid? id) : base(id ?? Guid.NewGuid()) 
    {
        Name = name;
        Address = address;
        Photo = photo;
        Birthday = birthday;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Owner() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    
}