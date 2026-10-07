using MyMall.Common;
using MyMall.Enums;

namespace MyMall.Entities;

public class Sale : BaseEntity
{
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Completed;

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}