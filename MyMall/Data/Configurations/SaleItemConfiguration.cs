using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyMall.Entities;

namespace MyMall.Data.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("sale_items");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.Id).HasColumnName("id");

        builder.Property(si => si.SaleId)
            .HasColumnName("sale_id")
            .IsRequired();

        builder.Property(si => si.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(si => si.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(si => si.TotalPrice)
            .HasColumnName("total_price")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(si => si.CreatedAt).HasColumnName("created_at");
        builder.Property(si => si.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(si => si.Sale)
            .WithMany(s => s.SaleItems)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(si => si.Product)
            .WithMany(p => p.SaleItems)
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}