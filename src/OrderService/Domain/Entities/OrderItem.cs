using SharedKernel.Domain;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

/// <summary>
/// Item do pedido - cada produto que o cliente comprou
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string ProductSku { get; private set; } = string.Empty;
    public Money Price { get; private set; } = Money.Zero;
    public int Quantity { get; private set; }
    
    // Navigation property pro EF
    public Order Order { get; private set; } = null!;

    // EF Core constructor
    private OrderItem() { }

    public static OrderItem Create(
        Guid productId,
        string productName,
        string productSku,
        Money price,
        int quantity)
    {
        // Validações básicas - melhor falhar cedo que depois
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));
        
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required", nameof(productName));
        
        if (string.IsNullOrWhiteSpace(productSku))
            throw new ArgumentException("Product SKU is required", nameof(productSku));
        
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        return new OrderItem
        {
            ProductId = productId,
            ProductName = productName.Trim(),
            ProductSku = productSku.Trim().ToUpperInvariant(), // SKU sempre em maiúscula
            Price = price,
            Quantity = quantity
        };
    }

    // Método pra calcular subtotal - útil pra relatórios
    public Money GetSubtotal() => new(Price.Amount * Quantity);

    // Método pra atualizar quantidade - caso o cliente mude antes de finalizar
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
        MarkAsUpdated();
    }
}