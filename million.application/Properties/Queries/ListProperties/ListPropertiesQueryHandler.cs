using MediatR;
using million.domain.properties;

namespace million.application.Properties.Queries.ListProperties;

public class ListPropertiesQueryHandler() : IRequestHandler<ListPropertiesQuery, List<Property>>
{
    public Task<List<Property>> Handle(ListPropertiesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}