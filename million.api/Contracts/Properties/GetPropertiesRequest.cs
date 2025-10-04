namespace million.api.Contracts.Properties;

public record GetPropertiesRequest
{
  public string? Name { get; set; } = string.Empty;
  public string? Address { get; set; } = string.Empty;
  public decimal? Price { get; set; }
  public int? Limit { get; set; } = 10;
  public int? Offset { get; set; } = 0;
}