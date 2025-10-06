namespace million.api.DTOs.Properties;

public record GetPropertiesRequest
{
  public string? Name { get; set; }
  public string? Address { get; set; }
  public decimal? MinPrice { get; set; }
  public decimal? MaxPrice { get; set; }
}