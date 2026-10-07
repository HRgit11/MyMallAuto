namespace MyMall.Services.DTOs;

using MyMall.Enums;

public class CreateSaleDto
{
    public int UserId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public List<CreateSaleItemDto> Items { get; set; } = new();
}

public class CreateSaleItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}