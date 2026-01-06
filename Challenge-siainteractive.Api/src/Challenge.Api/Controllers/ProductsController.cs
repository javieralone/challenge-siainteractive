using Challenge.Api.Models.ProductCategories;
using Challenge.Api.Models.Products;
using Challenge.Commands.Products.AssignCategory;
using Challenge.Commands.Products.Create;
using Challenge.Commands.Products.RemoveCategory;
using Challenge.Commands.Products.Update;
using Challenge.Commands.Products.UploadImage;
using Challenge.Queries.Common.Models;
using Challenge.Queries.ProductCategories.GetAll;
using Challenge.Queries.ProductCategories.Models;
using Challenge.Queries.Products.GetAll;
using Challenge.Queries.Products.GetById;
using Challenge.Queries.Products.Models;
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

    /// <summary>
    /// Upload image for a Product
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="file"></param>
    /// <returns>Product Id and Image URL</returns>
    [HttpPost("{productId}/image")]
    [ProducesResponseType(typeof(UploadProductImageCommandResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> UploadImage(long productId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        var request = new UploadProductImageCommandRequest(
            productId,
            file.OpenReadStream(),
            file.FileName,
            file.ContentType);

        var response = await _mediator.Send(request);

        return Ok(response);
    }

    /// <summary>
    /// Get Product by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Product details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetProductByIdQueryResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetById(long id)
    {
        var request = new GetProductByIdQueryRequest(Id: id);
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    /// <summary>
    /// Get all Products with pagination
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Paginated list of Products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetProductsQueryResponse), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAll([FromQuery] GetProductsRequest request)
    {
        var queryRequest = new GetProductsQueryRequest
        {
            Pagination = new PaginationRequest
            {
                PageNumber = request.PageNumber,
                RecordsPerPage = request.RecordsPerPage
            }
        };

        if (!string.IsNullOrWhiteSpace(request.SortBy) &&
            Enum.TryParse<ProductsOrderBy>(request.SortBy, true, out var orderBy))
        {
            queryRequest.OrderBy = new OrderFieldRequest<ProductsOrderBy>
            {
                OrderBy = orderBy,
                Direction = request.SortDirection
            };
        }

        var response = await _mediator.Send(queryRequest);

        return Ok(response);
    }

    /// <summary>
    /// Get Product Categories with optional filters
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Paginated list of Product Categories</returns>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(GetProductCategoriesQueryResponse), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetProductCategories([FromQuery] GetProductCategoriesRequest request)
    {
        var queryRequest = new GetProductCategoriesQueryRequest
        {
            ProductId = request.ProductId,
            CategoryId = request.CategoryId,
            Pagination = new PaginationRequest
            {
                PageNumber = request.PageNumber,
                RecordsPerPage = request.RecordsPerPage
            }
        };

        if (!string.IsNullOrWhiteSpace(request.SortBy) &&
            Enum.TryParse<ProductCategoriesOrderBy>(request.SortBy, true, out var orderBy))
        {
            queryRequest.OrderBy = new OrderFieldRequest<ProductCategoriesOrderBy>
            {
                OrderBy = orderBy,
                Direction = request.SortDirection
            };
        }

        var response = await _mediator.Send(queryRequest);

        return Ok(response);
    }
}

