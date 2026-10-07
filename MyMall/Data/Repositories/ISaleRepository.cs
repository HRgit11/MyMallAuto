using MyMall.Entities;

namespace MyMall.Data.Repositories;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetWithItemsAsync(int saleId);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<IEnumerable<Sale>> GetByUserAsync(int userId);
    Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to);
}