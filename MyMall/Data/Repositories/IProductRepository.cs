using MyMall.Entities;

namespace MyMall.Data.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
    Task<Product?> GetByBarcodeAsync(string barcode);
    Task<IEnumerable<Product>> GetLowStockAsync(int threshold);
    Task<IEnumerable<Product>> SearchAsync(string query);
}