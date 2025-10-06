namespace million.application.Properties.Queries.ListProperties;

public record PropertyResult(
    Guid IdOwner,
    string Name,
    string Address,
    decimal Price,
    List<string> Images
);
