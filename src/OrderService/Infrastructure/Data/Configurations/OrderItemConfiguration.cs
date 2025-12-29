using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do EF Core pra OrderItem
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        // Chave primária
        builder.HasKey(i => i.Id);

        // Propriedades
        builder.Property(i => i.ProductId)
            .IsRequired();

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.ProductSku)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Quantity)
            .IsRequired();

        // Configuração do Value Object Money pro preço
        builder.OwnsOne(i => i.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Propriedades de auditoria
        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        // Relacionamento com Order (já configurado no OrderConfiguration)
        builder.HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(i => i.ProductId)
            .HasDatabaseName("IX_OrderItems_ProductId");

        builder.HasIndex(i => i.ProductSku)
            .HasDatabaseName("IX_OrderItems_ProductSku");

        // Ignorar domain events
        builder.Ignore(i => i.DomainEvents);
    }
}