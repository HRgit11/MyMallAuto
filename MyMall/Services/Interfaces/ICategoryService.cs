using MyMall.Services.DTOs;

namespace MyMall.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CategoryDto dto);
    Task UpdateAsync(int id, CategoryDto dto);
    Task DeleteAsync(int id);
}