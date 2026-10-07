using Microsoft.EntityFrameworkCore;
using MyMall.Entities;

namespace MyMall.Data.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Barcode == barcode && p.IsActive);
    }

    public async Task<IEnumerable<Product>> GetLowStockAsync(int threshold)
    {
        return await _dbSet
            .Where(p => p.Quantity <= threshold && p.IsActive)
            .OrderBy(p => p.Quantity)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string query)
    {
        return await _dbSet
            .Where(p => p.IsActive &&
                       (p.Name.Contains(query) ||
                        (p.Barcode != null && p.Barcode.Contains(query))))
            .Take(50)
            .ToListAsync();
    }
}