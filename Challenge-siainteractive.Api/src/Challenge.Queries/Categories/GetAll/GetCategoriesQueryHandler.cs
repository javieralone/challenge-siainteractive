using Challenge.Infrastructure.Data.Persistence;
using Challenge.Queries.Categories.Models;
using Challenge.Queries.Common.Models;
using Challenge.Queries.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Queries.Categories.GetAll;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQueryRequest, GetCategoriesQueryResponse>
{
    private readonly ChallengeDBContext _dbContext;

    public GetCategoriesQueryHandler(ChallengeDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetCategoriesQueryResponse> Handle(GetCategoriesQueryRequest request, CancellationToken cancellationToken)
    {
        var query = CreateQuery(request.SearchTerm);

        query = CreateOrderByQuery(query, request);

        var totalRecords = await query.CountAsync();

        query = CreateOrderPagination(query, request);

        var data = await query.ToListAsync();

        var result = new PaginatedDataResponse<CategoriesDto>
        {
            PageNumber = request.Pagination?.PageNumber,
            RecordsPerPage = request.Pagination?.RecordsPerPage,
            Results = data,
            TotalRecords = totalRecords
        };

        return new GetCategoriesQueryResponse
        {
            Result = result
        };
    }

    private IQueryable<CategoriesDto> CreateQuery(string searchTerm)
    {
        var query = _dbContext.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query.Where(x => x.Name.ToUpper().Contains(searchTerm.ToUpper()));
        }

        return query.Select(x => new CategoriesDto
        {
            Id = x.Id,
            Name = x.Name
        });
    }

    private IQueryable<CategoriesDto> CreateOrderByQuery(IQueryable<CategoriesDto> query, GetCategoriesQueryRequest request)
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

    private IQueryable<CategoriesDto> CreateOrderPagination(IQueryable<CategoriesDto> query, GetCategoriesQueryRequest request)
    {
        if (request.Pagination == null)
        {
            return query;
        }

        return query.Skip(request.Pagination.SkipRecords).Take(request.Pagination.RecordsPerPage);
    }
}

