using SharedKernel.Application;
using OrderService.Infrastructure.Repositories;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Queries;

/// <summary>
/// Handler pra buscar pedido por ID - com cache pra não bater no banco toda hora
/// </summary>
public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDto?>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
            
            if (order == null)
                return Result<OrderDto?>.Success(null);

            // Converte pra DTO - só os dados que o cliente precisa
            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                Status = order.Status.ToFriendlyString(),
                TotalAmount = order.TotalAmount.Amount,
                Currency = order.TotalAmount.Currency,
                CreatedAt = order.CreatedAt,
                PaidAt = order.PaidAt,
                ShippedAt = order.ShippedAt,
                DeliveredAt = order.DeliveredAt,
                TrackingCode = order.TrackingCode,
                ShippingAddress = new AddressDto
                {
                    Street = order.ShippingAddress.Street,
                    Number = order.ShippingAddress.Number,
                    Complement = order.ShippingAddress.Complement,
                    Neighborhood = order.ShippingAddress.Neighborhood,
                    City = order.ShippingAddress.City,
                    State = order.ShippingAddress.State,
                    ZipCode = order.ShippingAddress.ZipCode,
                    Country = order.ShippingAddress.Country
                },
                Items = order.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductSku = item.ProductSku,
                    Price = item.Price.Amount,
                    Quantity = item.Quantity,
                    Subtotal = item.GetSubtotal().Amount
                }).ToList()
            };

            return Result<OrderDto?>.Success(orderDto);
        }
        catch (Exception ex)
        {
            return Result<OrderDto?>.Failure($"Failed to get order: {ex.Message}");
        }
    }
}