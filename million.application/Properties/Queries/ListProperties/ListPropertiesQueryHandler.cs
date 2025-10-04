using MediatR;
using million.domain.properties;
using million.domain.properties.specifications;

namespace million.application.Properties.Queries.ListProperties;

public class ListPropertiesQueryHandler(IPropertyRepository repository) : IRequestHandler<ListPropertiesQuery, List<Property>>
{
    public async Task<List<Property>> Handle(ListPropertiesQuery request, CancellationToken cancellationToken)
    {
        var spec = new PropertySpecificationBuilder()
            .WithName(request.Name)
            .WithAddress(request.Address)
            .WithRangePrice(request.MinPrice, request.MaxPrice)
            .Build(); 

        return await repository.FindAsync(spec, cancellationToken);
    }
}