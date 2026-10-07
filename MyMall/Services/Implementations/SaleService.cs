using MyMall.Data.Repositories;
using MyMall.Entities;
using MyMall.Enums;
using MyMall.Services.DTOs;
using MyMall.Services.Interfaces;

namespace MyMall.Services.Implementations;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;

    public SaleService(
        ISaleRepository saleRepository,
        IProductRepository productRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
    }

    public async Task<SaleDto> CreateSaleAsync(CreateSaleDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            throw new ArgumentException("Продажа должна содержать хотя бы один товар");

        var sale = new Sale
        {
            UserId = dto.UserId,
            PaymentMethod = dto.PaymentMethod,
            SaleDate = DateTime.UtcNow,
            Status = SaleStatus.Completed
        };

        decimal total = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId)
                ?? throw new ArgumentException($"Товар с ID {item.ProductId} не найден");

            if (!product.IsActive)
                throw new ArgumentException($"Товар '{product.Name}' недоступен");

            if (product.Quantity < item.Quantity)
                throw new ArgumentException($"Недостаточно товара '{product.Name}' на складе. Доступно: {product.Quantity}");

            var saleItem = new SaleItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * item.Quantity
            };

            sale.SaleItems.Add(saleItem);
            total += saleItem.TotalPrice;
            product.Quantity -= item.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        sale.TotalAmount = total;

        var created = await _saleRepository.AddAsync(sale);
        return MapToDto(created);
    }

    public async Task<SaleDto?> GetByIdAsync(int id)
    {
        var sale = await _saleRepository.GetWithItemsAsync(id);
        return sale == null ? null : MapToDto(sale);
    }

    public async Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var sales = await _saleRepository.GetByDateRangeAsync(from, to);
        return sales.Select(MapToDto);
    }

    public async Task<decimal> GetRevenueAsync(DateTime from, DateTime to)
    {
        return await _saleRepository.GetTotalRevenueAsync(from, to);
    }

    public async Task CancelSaleAsync(int saleId)
    {
        var sale = await _saleRepository.GetWithItemsAsync(saleId)
            ?? throw new ArgumentException("Продажа не найдена");

        if (sale.Status != SaleStatus.Completed)
            throw new ArgumentException("Можно отменить только завершённую продажу");
        foreach (var item in sale.SaleItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.Quantity += item.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        sale.Status = SaleStatus.Cancelled;
        await _saleRepository.UpdateAsync(sale);
    }

    private static SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            SaleDate = sale.SaleDate,
            CashierName = sale.User?.FullName ?? string.Empty,
            TotalAmount = sale.TotalAmount,
            PaymentMethod = sale.PaymentMethod,
            Status = sale.Status,
            Items = sale.SaleItems.Select(si => new SaleItemDto
            {
                ProductName = si.Product?.Name ?? string.Empty,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TotalPrice = si.TotalPrice
            }).ToList()
        };
    }
}