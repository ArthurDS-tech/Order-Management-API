using SharedKernel.Application;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Commands;

/// <summary>
/// Comando pra atualizar status do pedido - usado pelos outros serviços
/// </summary>
public class UpdateOrderStatusCommand : ICommand
{
    public Guid OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
    public string? TrackingCode { get; set; } // Só pra quando marcar como enviado
    public string? PaymentId { get; set; } // Só pra quando marcar como pago
    public string? CancellationReason { get; set; } // Só pra cancelamento
}