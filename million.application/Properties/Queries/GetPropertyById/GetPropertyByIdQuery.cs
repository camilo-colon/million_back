using MediatR;

namespace million.application.Properties.Queries.GetPropertyById;

public record GetPropertyByIdQuery(Guid PropertyId) : IRequest<PropertyDetailResult?>;
