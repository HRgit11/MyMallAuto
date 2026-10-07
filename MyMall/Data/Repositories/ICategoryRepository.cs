using MyMall.Entities;

namespace MyMall.Data.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    Task<bool> IsNameTakenAsync(string name, int? excludeId = null);
}