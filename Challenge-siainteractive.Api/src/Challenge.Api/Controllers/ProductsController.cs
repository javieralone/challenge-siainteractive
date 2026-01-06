using Challenge.Commands.Products.AssignCategory;
using Challenge.Commands.Products.Create;
using Challenge.Commands.Products.RemoveCategory;
using Challenge.Commands.Products.Update;
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


    /// <summary>
    /// Update existing Product
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Product Id</returns>
    [HttpPut]
    [ProducesResponseType(typeof(UpdateProductCommandResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> Update([FromBody] UpdateProductCommandRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    /// <summary>
    /// Assign a Category to a Product
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Product and Category Ids</returns>
    [HttpPost("{productId}/categories/{categoryId}")]
    [ProducesResponseType(typeof(AssignCategoryToProductCommandResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> AssignCategory(long productId, long categoryId)
    {
        var request = new AssignCategoryToProductCommandRequest(productId, categoryId);
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    /// <summary>
    /// Remove a Category from a Product
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="categoryId"></param>
    /// <returns>Product and Category Ids</returns>
    [HttpDelete("{productId}/categories/{categoryId}")]
    [ProducesResponseType(typeof(RemoveCategoryFromProductCommandResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> RemoveCategory(long productId, long categoryId)
    {
        var request = new RemoveCategoryFromProductCommandRequest(productId, categoryId);
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}

