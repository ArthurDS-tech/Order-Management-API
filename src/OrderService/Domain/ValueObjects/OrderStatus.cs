namespace OrderService.Domain.ValueObjects;

/// <summary>
/// Status do pedido - enum simples mas com significado claro
/// </summary>
public enum OrderStatus
{
    Pending = 1,    // Criado, esperando pagamento
    Paid = 2,       // Pago, pode ser processado
    Shipped = 3,    // Enviado, a caminho do cliente
    Delivered = 4,  // Entregue, tudo certo
    Cancelled = 5   // Cancelado por algum motivo
}

/// <summary>
/// Extensions pra facilitar o trabalho com status
/// </summary>
public static class OrderStatusExtensions
{
    public static string ToFriendlyString(this OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Aguardando Pagamento",
        OrderStatus.Paid => "Pago",
        OrderStatus.Shipped => "Enviado",
        OrderStatus.Delivered => "Entregue",
        OrderStatus.Cancelled => "Cancelado",
        _ => "Status Desconhecido"
    };

    public static bool CanTransitionTo(this OrderStatus current, OrderStatus target)
    {
        // Regras de transição - nem toda mudança de status faz sentido
        return current switch
        {
            OrderStatus.Pending => target is OrderStatus.Paid or OrderStatus.Cancelled,
            OrderStatus.Paid => target is OrderStatus.Shipped or OrderStatus.Cancelled,
            OrderStatus.Shipped => target is OrderStatus.Delivered,
            OrderStatus.Delivered => false, // Entregue é final
            OrderStatus.Cancelled => false, // Cancelado é final
            _ => false
        };
    }

    public static bool IsFinalStatus(this OrderStatus status) =>
        status is OrderStatus.Delivered or OrderStatus.Cancelled;
}