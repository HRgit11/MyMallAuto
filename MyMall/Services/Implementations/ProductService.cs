using MyMall.Data.Repositories;
using MyMall.Entities;
using MyMall.Services.DTOs;
using MyMall.Services.Interfaces;

namespace MyMall.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Where(p => p.IsActive).Select(MapToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllAsync();

        var products = await _productRepository.SearchAsync(query);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Название товара обязательно");

        if (dto.Price < 0)
            throw new ArgumentException("Цена не может быть отрицательной");

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Barcode = dto.Barcode,
            Price = dto.Price,
            PurchasePrice = dto.PurchasePrice,
            Quantity = dto.Quantity,
            CategoryId = dto.CategoryId,
            IsActive = true
        };

        var created = await _productRepository.AddAsync(product);
        return MapToDto(created);
    }

    public async Task UpdateAsync(int id, CreateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new ArgumentException("Товар не найден");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Barcode = dto.Barcode;
        product.Price = dto.Price;
        product.PurchasePrice = dto.PurchasePrice;
        product.Quantity = dto.Quantity;
        product.CategoryId = dto.CategoryId;

        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new ArgumentException("Товар не найден");

        product.IsActive = false;
        await _productRepository.UpdateAsync(product);
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockAsync(int threshold = 10)
    {
        var products = await _productRepository.GetLowStockAsync(threshold);
        return products.Select(MapToDto);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Barcode = product.Barcode,
            Price = product.Price,
            PurchasePrice = product.PurchasePrice,
            Quantity = product.Quantity,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            IsActive = product.IsActive
        };
    }
}