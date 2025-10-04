using million.domain.common;

namespace million.domain.properties;

public class Property : Entity
{
    
    private readonly List<Guid> _imagesIds = [];
    
    private readonly List<Guid> _tracesIds = [];
    
    public Guid OwnerId { get; private init; }
    public string Name { get; private init; }
    public string Address { get; private init; }
    public decimal Price { get; private init; }
    public string CodeInternal { get; private init; }
    public int Year { get; private init; }
    public IReadOnlyList<Guid> ImagesIds => _imagesIds.AsReadOnly();
    public IReadOnlyList<Guid> TracesIds => _tracesIds.AsReadOnly();

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
    
    public Property() {}
}