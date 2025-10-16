namespace million.application.Properties.Queries.GetPropertyById;

public record PropertyDetailResult(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Address,
    decimal Price,
    string CodeInternal,
    int Year,
    List<PropertyImageResult> Images
);

public record PropertyImageResult(
    Guid Id,
    string File,
    bool Enabled
);
