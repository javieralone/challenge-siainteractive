using Challenge.Domain.Entities;
using Challenge.Infrastructure.Data.Persistence;
using Challenge.Queries.ProductCategories.Models;
using Challenge.Queries.Common.Models;
using Challenge.Queries.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Queries.ProductCategories.GetAll;

public class GetProductCategoriesQueryHandler : IRequestHandler<GetProductCategoriesQueryRequest, GetProductCategoriesQueryResponse>
{
    private readonly ChallengeDBContext _dbContext;

    public GetProductCategoriesQueryHandler(ChallengeDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetProductCategoriesQueryResponse> Handle(GetProductCategoriesQueryRequest request, CancellationToken cancellationToken)
    {
        var entityQuery = _dbContext.ProductCategories.AsQueryable();

        entityQuery = ApplyFilters(entityQuery, request);

        var totalRecords = await entityQuery.CountAsync(cancellationToken);

        entityQuery = CreateOrderByQuery(entityQuery, request);

        entityQuery = CreateOrderPagination(entityQuery, request);

        var data = await entityQuery
            .Select(pc => new ProductCategoriesDto
            {
                Id = pc.Id,
                ProductId = pc.ProductId,
                ProductName = pc.Product.Name,
                CategoryId = pc.CategoryId,
                CategoryName = pc.Category.Name
            })
            .ToListAsync(cancellationToken);

        var result = new PaginatedDataResponse<ProductCategoriesDto>
        {
            PageNumber = request.Pagination?.PageNumber,
            RecordsPerPage = request.Pagination?.RecordsPerPage,
            Results = data,
            TotalRecords = totalRecords
        };

        return new GetProductCategoriesQueryResponse
        {
            Result = result
        };
    }

    private IQueryable<ProductCategory> ApplyFilters(IQueryable<ProductCategory> query, GetProductCategoriesQueryRequest request)
    {
        if (request.ProductId.HasValue)
        {
            query = query.Where(pc => pc.ProductId == request.ProductId.Value);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(pc => pc.CategoryId == request.CategoryId.Value);
        }

        return query;
    }

    private IQueryable<ProductCategory> CreateOrderByQuery(IQueryable<ProductCategory> query, GetProductCategoriesQueryRequest request)
    {
        if (request.OrderBy == null)
        {
            return query.OrderBy(pc => pc.Id);
        }

        query = request.OrderBy.OrderBy switch
        {
            ProductCategoriesOrderBy.ProductId => request.OrderBy.Direction == OrderFieldQueryDirection.ASC
                ? query.OrderBy(pc => pc.ProductId)
                : query.OrderByDescending(pc => pc.ProductId),
            ProductCategoriesOrderBy.CategoryId => request.OrderBy.Direction == OrderFieldQueryDirection.ASC
                ? query.OrderBy(pc => pc.CategoryId)
                : query.OrderByDescending(pc => pc.CategoryId),
            ProductCategoriesOrderBy.ProductName => request.OrderBy.Direction == OrderFieldQueryDirection.ASC
                ? query.OrderBy(pc => pc.Product.Name)
                : query.OrderByDescending(pc => pc.Product.Name),
            ProductCategoriesOrderBy.CategoryName => request.OrderBy.Direction == OrderFieldQueryDirection.ASC
                ? query.OrderBy(pc => pc.Category.Name)
                : query.OrderByDescending(pc => pc.Category.Name),
            _ => query.OrderBy(pc => pc.Id)
        };

        return query;
    }

    private IQueryable<ProductCategory> CreateOrderPagination(IQueryable<ProductCategory> query, GetProductCategoriesQueryRequest request)
    {
        if (request.Pagination == null)
        {
            return query;
        }

        return query.Skip(request.Pagination.SkipRecords).Take(request.Pagination.RecordsPerPage);
    }
}

