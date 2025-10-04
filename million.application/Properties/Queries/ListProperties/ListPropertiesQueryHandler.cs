using MediatR;
using million.domain.properties;

namespace million.application.Properties.Queries.ListProperties;

public class ListPropertiesQueryHandler(IPropertyRepository repository) : IRequestHandler<ListPropertiesQuery, List<Property>>
{
    public async Task<List<Property>> Handle(ListPropertiesQuery request, CancellationToken cancellationToken)
    {
        return await repository.Find();
    }
}