using MediatR;
using Microsoft.AspNetCore.Mvc;
using million.api.Contracts.Properties;
using million.application.Properties.Queries.ListProperties;
using million.domain.properties;

namespace million.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController(ISender sender, IPropertyRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] GetPropertiesRequest request)
    {
        var query = new ListPropertiesQuery
        {
            Name = request.Name,
            Address = request.Address,
            Price = request.Price,
            Limit = request.Limit,
            Offset = request.Offset
        };
        var response = await sender.Send(query);
        return Ok(response);
    }
}