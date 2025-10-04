namespace million.domain.properties;

public interface IPropertyRepository
{
    Task<List<Property>> Find();
}