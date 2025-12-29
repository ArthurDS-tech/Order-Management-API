using SharedKernel.Application;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Commands;

/// <summary>
/// Comando pra criar um novo pedido - tudo que precisa pra montar um pedido
/// </summary>
public class CreateOrderCommand : ICommand<Guid>
{
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public AddressDto ShippingAddress { get; set; } = new();
    public List<OrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO pro endereço - dados que vem do frontend
/// </summary>
public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = "Brasil";

    // Método pra converter pro Value Object
    public Address ToValueObject() => new(
        Street, Number, Neighborhood, City, State, ZipCode, Country, Complement);
}

/// <summary>
/// DTO pro item do pedido
/// </summary>
public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}