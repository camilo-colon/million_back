using MediatR;
using million.application.Common.Request;
using million.domain.properties;

namespace million.application.Properties.Queries.ListProperties;

public record ListPropertiesQuery : IRequest<List<Property>>
{ 
    public string? Name { get; set; }
    public string? Address { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}