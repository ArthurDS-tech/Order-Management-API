using SharedKernel.Application;
using OrderService.Domain.ValueObjects;
using OrderService.Infrastructure.Repositories;
using MediatR;

namespace OrderService.Application.Commands;

/// <summary>
/// Handler pra atualizar status - cada status tem sua lógica específica
/// </summary>
public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMediator _mediator;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository, IMediator mediator)
    {
        _orderRepository = orderRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
                return Result.Failure("Order not found");

            // Aplica a mudança de status baseado no que foi pedido
            switch (request.NewStatus)
            {
                case OrderStatus.Paid:
                    if (string.IsNullOrEmpty(request.PaymentId))
                        return Result.Failure("Payment ID is required when marking as paid");
                    
                    order.MarkAsPaid(request.PaymentId);
                    break;

                case OrderStatus.Shipped:
                    if (string.IsNullOrEmpty(request.TrackingCode))
                        return Result.Failure("Tracking code is required when marking as shipped");
                    
                    order.MarkAsShipped(request.TrackingCode);
                    break;

                case OrderStatus.Delivered:
                    order.MarkAsDelivered();
                    break;

                case OrderStatus.Cancelled:
                    var reason = request.CancellationReason ?? "No reason provided";
                    order.Cancel(reason);
                    break;

                default:
                    return Result.Failure($"Invalid status transition to {request.NewStatus}");
            }

            // Salva as mudanças
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            // Publica os eventos - notification service vai escutar
            foreach (var domainEvent in order.DomainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            order.ClearDomainEvents();

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            // Transição de status inválida
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update order status: {ex.Message}");
        }
    }
}