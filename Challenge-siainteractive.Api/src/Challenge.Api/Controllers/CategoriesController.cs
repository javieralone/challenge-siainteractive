using Challenge.Commands.Categories.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Challenge.Api.Controllers;

/// <summary>
/// Category Controller
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class CategoriesController : Controller
{
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create new Category
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Category Id</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCategoryCommandResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommandRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    /// <summary>
    /// Update existing Category
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Category Id</returns>
    [HttpPut]
    [ProducesResponseType(typeof(UpdateCategoryCommandResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> Update([FromBody] UpdateCategoryCommandRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}