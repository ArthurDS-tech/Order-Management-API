using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;

namespace OrderService.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do EF Core pra entidade Order - aqui definimos como mapear pro banco
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        // Chave primária
        builder.HasKey(o => o.Id);

        // Propriedades básicas
        builder.Property(o => o.CustomerEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.CustomerName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>(); // Salva como string no banco

        builder.Property(o => o.TrackingCode)
            .HasMaxLength(100);

        // Configuração do Value Object Money
        builder.OwnsOne(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Configuração do Value Object Address
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("ShippingStreet")
                .HasMaxLength(255)
                .IsRequired();

            address.Property(a => a.Number)
                .HasColumnName("ShippingNumber")
                .HasMaxLength(20)
                .IsRequired();

            address.Property(a => a.Complement)
                .HasColumnName("ShippingComplement")
                .HasMaxLength(255);

            address.Property(a => a.Neighborhood)
                .HasColumnName("ShippingNeighborhood")
                .HasMaxLength(255)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("ShippingCity")
                .HasMaxLength(255)
                .IsRequired();

            address.Property(a => a.State)
                .HasColumnName("ShippingState")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.ZipCode)
                .HasColumnName("ShippingZipCode")
                .HasMaxLength(20)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("ShippingCountry")
                .HasMaxLength(100)
                .IsRequired();
        });

        // Relacionamento com OrderItems
        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        // Propriedades de auditoria
        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt);

        builder.Property(o => o.CreatedBy)
            .HasMaxLength(255);

        builder.Property(o => o.UpdatedBy)
            .HasMaxLength(255);

        // Índices pra performance - baseado nas queries mais comuns
        builder.HasIndex(o => o.CustomerEmail)
            .HasDatabaseName("IX_Orders_CustomerEmail");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("IX_Orders_Status");

        builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("IX_Orders_CreatedAt");

        // Índice composto pra queries com filtros múltiplos
        builder.HasIndex(o => new { o.Status, o.CreatedAt })
            .HasDatabaseName("IX_Orders_Status_CreatedAt");

        // Ignorar propriedades que não vão pro banco
        builder.Ignore(o => o.DomainEvents);
    }
}