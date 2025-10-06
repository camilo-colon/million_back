using MediatR;

namespace million.application.Properties.Queries.ListProperties;

public record ListPropertiesQuery : IRequest<List<PropertyResult>>
{ 
    public string? Name { get; set; }
    public string? Address { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}