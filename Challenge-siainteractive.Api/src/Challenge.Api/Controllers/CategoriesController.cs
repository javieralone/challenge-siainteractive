using Challenge.Api.Models.Categories;
using Challenge.Commands.Categories.Create;
using Challenge.Commands.Categories.Update;
using Challenge.Queries.Categories.GetAll;
using Challenge.Queries.Categories.GetById;
using Challenge.Queries.Categories.Models;
using Challenge.Queries.Common.Models;
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

    /// <summary>
    /// Get Category by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Category details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetCategoryByIdQueryResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetById(long id)
    {
        var request = new GetCategoryByIdQueryRequest(Id: id);
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    /// <summary>
    /// Get all Categories with pagination
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Paginated list of Categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetCategoriesQueryResponse), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAll([FromQuery] GetCategoriesRequest request)
    {
        var queryRequest = new GetCategoriesQueryRequest
        {
            Pagination = new PaginationRequest
            {
                PageNumber = request.PageNumber,
                RecordsPerPage = request.RecordsPerPage
            }
        };

        if (!string.IsNullOrWhiteSpace(request.SortBy) &&
            Enum.TryParse<CategoriesOrderBy>(request.SortBy, true, out var orderBy))
        {
            queryRequest.OrderBy = new OrderFieldRequest<CategoriesOrderBy>
            {
                OrderBy = orderBy,
                Direction = request.SortDirection
            };
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            queryRequest.SearchTerm = request.SearchTerm;
        }

        var response = await _mediator.Send(queryRequest);

        return Ok(response);
    }
}