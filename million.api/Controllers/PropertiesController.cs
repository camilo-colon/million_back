using MediatR;
using Microsoft.AspNetCore.Mvc;
using million.api.DTOs.Properties;
using million.application.Properties.Queries.ListProperties;

namespace million.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] GetPropertiesRequest request)
    {
        var query = new ListPropertiesQuery
        {
            Name = request.Name,
            Address = request.Address,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice
        };
        var response = await sender.Send(query);
        return Ok(response);
    }
}