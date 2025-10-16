using MediatR;
using million.domain.properties;
using million.domain.properties.specifications;
using million.domain.PropertyImages;
using million.domain.PropertyImages.Specifications;

namespace million.application.Properties.Queries.ListProperties;

public class ListPropertiesQueryHandler(IPropertyRepository propertyRepository, IPropertyImageRepository imageRepository) : IRequestHandler<ListPropertiesQuery, List<PropertyResult>>
{
    public async Task<List<PropertyResult>> Handle(ListPropertiesQuery request, CancellationToken cancellationToken)
    {
        var spec = new PropertySpecificationBuilder()
            .WithName(request.Name)
            .WithAddress(request.Address)
            .WithMinPrice(request.MinPrice)
            .WithMaxPrice(request.MaxPrice)
            .Build();
        
        var properties = await propertyRepository.GetBySpec(spec, cancellationToken);
        
        var result = new List<PropertyResult>();

        foreach (var property in properties)
        {
            var images = await imageRepository.GetBySpec(new PropertyImageByPropertyIdSpec(property.Id), cancellationToken);

            result.Add(new PropertyResult(
                property.Id,
                property.OwnerId,
                property.Name,
                property.Address,
                property.Price,
                images.Select(p => p.File).FirstOrDefault() ?? string.Empty
            ));
        }

        return result;
    }
}