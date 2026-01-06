using Challenge.Infrastructure.Data.Persistence;
using Challenge.Queries.Products.Models;
using Challenge.Queries.Common.Models;
using Challenge.Queries.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Queries.Products.GetAll;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQueryRequest, GetProductsQueryResponse>
{
    private readonly ChallengeDBContext _dbContext;

    public GetProductsQueryHandler(ChallengeDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetProductsQueryResponse> Handle(GetProductsQueryRequest request, CancellationToken cancellationToken)
    {
        var query = CreateQuery();

        query = CreateOrderByQuery(query, request);

        var totalRecords = await query.CountAsync(cancellationToken);

        query = CreateOrderPagination(query, request);

        var data = await query.ToListAsync(cancellationToken);

        var result = new PaginatedDataResponse<ProductsDto>
        {
            PageNumber = request.Pagination?.PageNumber,
            RecordsPerPage = request.Pagination?.RecordsPerPage,
            Results = data,
            TotalRecords = totalRecords
        };

        return new GetProductsQueryResponse
        {
            Result = result
        };
    }

    private IQueryable<ProductsDto> CreateQuery()
    {
        return _dbContext.Products.Select(x => new ProductsDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Image = x.Image != null ? x.Image.Value : null
        });
    }

    private IQueryable<ProductsDto> CreateOrderByQuery(IQueryable<ProductsDto> query, GetProductsQueryRequest request)
    {
        if (request.OrderBy == null)
        {
            return query;
        }

        if (request.OrderBy.Direction == OrderFieldQueryDirection.ASC)
        {
            return query.OrderBy(request.OrderBy.OrderBy.ToString());
        }

        return query.OrderByDescending(request.OrderBy.OrderBy.ToString());
    }

    private IQueryable<ProductsDto> CreateOrderPagination(IQueryable<ProductsDto> query, GetProductsQueryRequest request)
    {
        if (request.Pagination == null)
        {
            return query;
        }

        return query.Skip(request.Pagination.SkipRecords).Take(request.Pagination.RecordsPerPage);
    }
}

