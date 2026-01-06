using Challenge.Domain.Entities;
using LanguageExt;

namespace Challenge.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Option<Category>> Get(long courseId);
    Task Save(Category course);
    Task<Option<Category>> Validate(string name);
    Task<IQueryable<Category>> GetAll();
}
