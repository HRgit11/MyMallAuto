using Microsoft.EntityFrameworkCore;
using MyMall.Entities;

namespace MyMall.Data.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> IsNameTakenAsync(string name, int? excludeId = null)
    {
        return await _dbSet
            .AnyAsync(c => c.Name == name && (excludeId == null || c.Id != excludeId));
    }
}