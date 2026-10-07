using Microsoft.EntityFrameworkCore;
using MyMall.Entities;
using MyMall.Enums;

namespace MyMall.Data.Repositories;

public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Sale?> GetWithItemsAsync(int saleId)
    {
        return await _dbSet
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == saleId);
    }

    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _dbSet
            .Include(s => s.User)
            .Where(s => s.SaleDate >= from && s.SaleDate <= to)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByUserAsync(int userId)
    {
        return await _dbSet
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(s => s.SaleDate >= from &&
                        s.SaleDate <= to &&
                        s.Status == SaleStatus.Completed)
            .SumAsync(s => s.TotalAmount);
    }
}