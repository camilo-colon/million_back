using MediatR;
using million.domain.properties;
using million.domain.PropertyImages;
using million.domain.PropertyImages.Specifications;

namespace million.application.Properties.Queries.GetPropertyById;

public class GetPropertyByIdQueryHandler(
    IPropertyRepository propertyRepository,
    IPropertyImageRepository propertyImageRepository)
    : IRequestHandler<GetPropertyByIdQuery, PropertyDetailResult?>
{
    public async Task<PropertyDetailResult?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);

        if (property == null)
        {
            return null;
        }

        var imagesSpec = new PropertyImageByPropertyIdSpec(request.PropertyId);
        var images = await propertyImageRepository.GetBySpec(imagesSpec, cancellationToken);

        var imageResults = images.Select(img => new PropertyImageResult(
            img.Id,
            img.File,
            img.Enabled
        )).ToList();

        return new PropertyDetailResult(
            property.Id,
            property.OwnerId,
            property.Name,
            property.Address,
            property.Price,
            property.CodeInternal,
            property.Year,
            imageResults
        );
    }
}
