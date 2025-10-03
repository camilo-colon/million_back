using million.domain.common;

namespace million.domain.properties;

public class PropertyTrace : Entity
{
    public Guid PropertyId { get; }
    
    public DateTime DateSale { get; }
    
    public string Name { get; }
    
    public decimal Value { get; }
    
    public decimal Tax { get; }
}