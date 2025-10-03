using million.domain.common;

namespace million.domain.properties;

public class Property : Entity
{
    public Guid OwnerId { get; }
    
    public readonly List<PropertyImage> _images = [];

    public readonly List<PropertyTrace> _traces = [];
    public string Name { get; }
    public string Address { get; }
    public decimal Price { get; }
    public string CodeInternal { get; }
    public int Year { get; }

    public Property(
        Guid ownerId, 
        string name, 
        string address, 
        decimal price, 
        string codeInternal, 
        int year, 
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        OwnerId = ownerId;
        Name = name;
        Address = address;
        Price = price;
        CodeInternal = codeInternal;
        Year = year;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Property() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}