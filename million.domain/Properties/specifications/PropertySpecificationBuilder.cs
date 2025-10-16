using million.domain.Common.specifications;

namespace million.domain.properties.specifications;

public class PropertySpecificationBuilder
{
    private Specification<Property> _spec = Specification<Property>.All;

    public PropertySpecificationBuilder WithName(string? name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            _spec = _spec.And(new PropertyByNameSpec(name));
        }
        return this;
    }

    public PropertySpecificationBuilder WithAddress(string? address)
    {
        if (!string.IsNullOrEmpty(address))
        {
            _spec = _spec.And(new PropertyByAddressSpec(address));
        }
        return this;
    }

    public PropertySpecificationBuilder WithMinPrice(decimal? minPrice)
    {
        if (minPrice.HasValue)
        {
            _spec = _spec.And(new PropertyByMinPriceSpec(minPrice.Value));
        }
        return this; 
    }
    
    public PropertySpecificationBuilder WithMaxPrice(decimal? maxPrice)
    {
        if (maxPrice.HasValue)
        {
            _spec = _spec.And(new PropertyByMaxPriceSpec(maxPrice.Value));
        }
        return this; 
    } 

    public PropertySpecificationBuilder WithRangePrice(decimal? minPrice, decimal? maxPrice)
    {
        if (minPrice.HasValue && maxPrice.HasValue)
        {
            _spec = _spec.And(new PropertyByRangePriceSpec(minPrice.Value, maxPrice.Value));
        }
        
        return this;
    }

    public Specification<Property> Build() => _spec;
}