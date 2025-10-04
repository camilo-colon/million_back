using million.domain.Common.specifications;

namespace million.domain.properties;

public interface IPropertyRepository
{
    Task<List<Property>> FindAsync(ISpecification<Property> spec, CancellationToken token);
    Task<List<Property>>AllAsync(CancellationToken token);
}