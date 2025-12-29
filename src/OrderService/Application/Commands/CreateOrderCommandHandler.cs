using SharedKernel.Application;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using OrderService.Infrastructure.Repositories;
using MediatR;

namespace OrderService.Application.Commands;

/// <summary>
/// Handler que processa a criação de pedidos - aqui é onde a coisa acontece
/// </summary>
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMediator _mediator;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IMediator mediator)
    {
        _orderRepository = orderRepository;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Converte os DTOs pra objetos de domínio
            var address = request.ShippingAddress.ToValueObject();
            
            var orderItems = request.Items.Select(item => 
                OrderItem.Create(
                    item.ProductId,
                    item.ProductName,
                    item.ProductSku,
                    new Money(item.Price),
                    item.Quantity
                )).ToList();

            // Cria o pedido usando o factory method
            var order = Order.Create(
                request.CustomerEmail,
                request.CustomerName,
                address,
                orderItems
            );

            // Salva no banco
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            // Publica os eventos de domínio - outros serviços vão escutar
            foreach (var domainEvent in order.DomainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            order.ClearDomainEvents();

            return Result<Guid>.Success(order.Id);
        }
        catch (ArgumentException ex)
        {
            // Erros de validação - cliente mandou dados inválidos
            return Result<Guid>.Failure($"Invalid data: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Outros erros - algo deu errado no sistema
            return Result<Guid>.Failure($"Failed to create order: {ex.Message}");
        }
    }
}