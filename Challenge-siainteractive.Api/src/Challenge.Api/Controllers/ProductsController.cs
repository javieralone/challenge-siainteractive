using Challenge.Commands.Products.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Challenge.Api.Controllers;

/// <summary>
/// Product Controller
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : Controller
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create new Product
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Product Id</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateProductCommandResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommandRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }

}

