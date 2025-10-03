namespace million.domain.properties;

public interface IPropertyRepository
{
    Task<List<Property>> GetByFilters(string? name, string? address, string? price, int limit, int offset);
}