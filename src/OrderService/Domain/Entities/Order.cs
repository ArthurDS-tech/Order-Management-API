using SharedKernel.Domain;
using OrderService.Domain.ValueObjects;
using OrderService.Domain.Events;

namespace OrderService.Domain.Entities;

/// <summary>
/// Aggregate root do pedido - aqui é onde a mágica acontece
/// </summary>
public class Order : BaseEntity
{
    // Props privadas pra manter encapsulamento - ninguém muda o estado sem passar pelos métodos
    private readonly List<OrderItem> _items = new();
    
    public string CustomerEmail { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public Money TotalAmount { get; private set; } = Money.Zero;
    public Address ShippingAddress { get; private set; } = null!;
    public DateTime? PaidAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public string? TrackingCode { get; private set; }

    // Readonly collection pra não deixar ninguém mexer diretamente
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // EF Core precisa de um construtor sem parâmetros
    private Order() { }

    // Factory method - jeito mais limpo de criar um pedido
    public static Order Create(
        string customerEmail, 
        string customerName, 
        Address shippingAddress,
        List<OrderItem> items)
    {
        // Aqui a gente valida se faz sentido criar o pedido
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email is required", nameof(customerEmail));
        
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required", nameof(customerName));
        
        if (!items.Any())
            throw new ArgumentException("Order must have at least one item", nameof(items));

        var order = new Order
        {
            CustomerEmail = customerEmail.Trim().ToLowerInvariant(),
            CustomerName = customerName.Trim(),
            ShippingAddress = shippingAddress,
            Status = OrderStatus.Pending
        };

        // Adiciona os itens e calcula o total
        foreach (var item in items)
        {
            order._items.Add(item);
        }
        
        order.RecalculateTotal();

        // Dispara evento de pedido criado - outros serviços vão escutar isso
        order.AddDomainEvent(new OrderCreatedEvent(order.Id, order.CustomerEmail, order.TotalAmount.Amount));

        return order;
    }

    public void MarkAsPaid(string paymentId)
    {
        // Só pode pagar se tiver pendente
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot mark order as paid. Current status: {Status}");

        Status = OrderStatus.Paid;
        PaidAt = DateTime.UtcNow;
        MarkAsUpdated();

        // Evento pra avisar que foi pago - inventory service vai escutar
        AddDomainEvent(new OrderPaidEvent(Id, paymentId));
    }

    public void MarkAsShipped(string trackingCode)
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException($"Cannot ship order. Current status: {Status}");

        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
        TrackingCode = trackingCode;
        MarkAsUpdated();

        AddDomainEvent(new OrderShippedEvent(Id, CustomerEmail, trackingCode));
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException($"Cannot mark as delivered. Current status: {Status}");

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        MarkAsUpdated();

        AddDomainEvent(new OrderDeliveredEvent(Id, CustomerEmail));
    }

    public void Cancel(string reason)
    {
        // Só pode cancelar se não foi enviado ainda
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new InvalidOperationException($"Cannot cancel order. Current status: {Status}");

        Status = OrderStatus.Cancelled;
        MarkAsUpdated();

        AddDomainEvent(new OrderCancelledEvent(Id, CustomerEmail, reason));
    }

    private void RecalculateTotal()
    {
        var total = _items.Sum(item => item.Price.Amount * item.Quantity);
        TotalAmount = new Money(total);
    }

    // Helper pra adicionar item - útil pra testes ou casos especiais
    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify items after order is processed");

        _items.Add(item);
        RecalculateTotal();
        MarkAsUpdated();
    }
}