using million.application.Common.Request;
using million.domain.properties;

namespace million.application.Properties.Queries.ListProperties;

public record ListPropertiesQuery : Query<List<Property>>
{ 
    public string? Name { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    
}