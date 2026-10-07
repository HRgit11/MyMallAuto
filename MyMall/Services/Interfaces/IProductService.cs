using MyMall.Services.DTOs;

namespace MyMall.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> SearchAsync(string query);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task UpdateAsync(int id, CreateProductDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<ProductDto>> GetLowStockAsync(int threshold = 10);
}