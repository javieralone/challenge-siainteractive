using Challenge.Domain.Entities;
using Challenge.Domain.Repositories;
using Challenge.Infrastructure.Data.Persistence;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Infrastructure.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ChallengeDBContext _dbContext;

    public CategoryRepository(ChallengeDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Option<Category>> Get(long id)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task Save(Category category)
    {
        _dbContext.Categories.Attach(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Option<Category>> Validate(string name)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name.Equals(name));
    }

    public Task<IQueryable<Category>> GetAll()
    {
        return Task.FromResult(_dbContext.Categories.AsQueryable());
    }
}
