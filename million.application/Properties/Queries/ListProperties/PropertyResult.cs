namespace million.application.Properties.Queries.ListProperties;

public record PropertyResult(
    Guid Id,
    Guid IdOwner,
    string Name,
    string Address,
    decimal Price,
    string Image
);
