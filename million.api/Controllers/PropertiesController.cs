using MediatR;
using Microsoft.AspNetCore.Mvc;
using million.api.DTOs.Properties;
using million.application.Properties.Queries.GetPropertyById;
using million.application.Properties.Queries.ListProperties;

namespace million.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Index([FromBody] GetPropertiesRequest request)
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetPropertyByIdQuery(id);
        var response = await sender.Send(query);

        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}