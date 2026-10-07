using MyMall.Data.Repositories;
using MyMall.Entities;
using MyMall.Services.DTOs;
using MyMall.Services.Interfaces;

namespace MyMall.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetActiveCategoriesAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Название категории обязательно");

        if (await _categoryRepository.IsNameTakenAsync(dto.Name))
            throw new ArgumentException("Категория с таким названием уже существует");

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true
        };

        var created = await _categoryRepository.AddAsync(category);
        return MapToDto(created);
    }

    public async Task UpdateAsync(int id, CategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new ArgumentException("Категория не найдена");

        if (await _categoryRepository.IsNameTakenAsync(dto.Name, id))
            throw new ArgumentException("Категория с таким названием уже существует");

        category.Name = dto.Name;
        category.Description = dto.Description;

        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new ArgumentException("Категория не найдена");

        category.IsActive = false;
        await _categoryRepository.UpdateAsync(category);
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            ProductCount = category.Products?.Count ?? 0
        };
    }
}