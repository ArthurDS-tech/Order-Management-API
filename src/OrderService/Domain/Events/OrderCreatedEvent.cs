using SharedKernel.Domain;

namespace OrderService.Domain.Events;

/// <summary>
/// Evento disparado quando um pedido é criado - outros serviços vão escutar isso
/// </summary>
public class OrderCreatedEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string CustomerEmail { get; }
    public decimal TotalAmount { get; }

    public OrderCreatedEvent(Guid orderId, string customerEmail, decimal totalAmount)
    {
        OrderId = orderId;
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
    }
}

/// <summary>
/// Evento quando pedido é pago - inventory service precisa saber disso
/// </summary>
public class OrderPaidEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string PaymentId { get; }

    public OrderPaidEvent(Guid orderId, string paymentId)
    {
        OrderId = orderId;
        PaymentId = paymentId;
    }
}

/// <summary>
/// Evento quando pedido é enviado - cliente precisa ser notificado
/// </summary>
public class OrderShippedEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string CustomerEmail { get; }
    public string TrackingCode { get; }

    public OrderShippedEvent(Guid orderId, string customerEmail, string trackingCode)
    {
        OrderId = orderId;
        CustomerEmail = customerEmail;
        TrackingCode = trackingCode;
    }
}

/// <summary>
/// Evento quando pedido é entregue - fim da jornada
/// </summary>
public class OrderDeliveredEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string CustomerEmail { get; }

    public OrderDeliveredEvent(Guid orderId, string customerEmail)
    {
        OrderId = orderId;
        CustomerEmail = customerEmail;
    }
}

/// <summary>
/// Evento quando pedido é cancelado - precisa liberar estoque
/// </summary>
public class OrderCancelledEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string CustomerEmail { get; }
    public string Reason { get; }

    public OrderCancelledEvent(Guid orderId, string customerEmail, string reason)
    {
        OrderId = orderId;
        CustomerEmail = customerEmail;
        Reason = reason;
    }
}