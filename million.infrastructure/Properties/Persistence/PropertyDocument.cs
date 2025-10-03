using million.domain.properties;
using MongoDB.Bson.Serialization.Attributes;

namespace million.infrastructure.Properties.Persistence;

public class PropertyDocument(
  Guid ownerId,
  string name,
  string address,
  decimal price,
  string codeInternal,
  int year,
  Guid id)
{
  [BsonId] 
  public Guid Id { get; set; } = id;

  public Guid OwnerId { get; set; } = ownerId;

  public string Name { get; set; } = name;

  public string Address { get; set; } = address;

  public decimal Price { get; set; } = price;

  public string CodeInternal { get; set; } = codeInternal;
  
  public int Year { get; set; } = year;

  public static PropertyDocument ToDocument(Property property)
  {
    var document = new PropertyDocument(property.OwnerId, property.Name, property.Address, property.Price, property.CodeInternal, property.Year, property.Id);
    return document;
  }

  public Property ToModel()
  {
    return new Property(
      OwnerId, 
      Name, 
      Address, 
      Price, 
      CodeInternal, 
      Year, 
      Id
    );
  }
  
}