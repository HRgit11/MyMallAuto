using MyMall.Services.DTOs;

namespace MyMall.Services.Interfaces;

public interface ISaleService
{
    Task<SaleDto> CreateSaleAsync(CreateSaleDto dto);
    Task<SaleDto?> GetByIdAsync(int id);
    Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<decimal> GetRevenueAsync(DateTime from, DateTime to);
    Task CancelSaleAsync(int saleId);
}