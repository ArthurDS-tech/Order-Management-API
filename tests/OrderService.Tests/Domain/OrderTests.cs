using FluentAssertions;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using OrderService.Domain.Events;

namespace OrderService.Tests.Domain;

/// <summary>
/// Testes da entidade Order - aqui testamos as regras de negócio
/// </summary>
public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        var customerEmail = "test@example.com";
        var customerName = "João Silva";
        var address = CreateValidAddress();
        var items = new List<OrderItem>
        {
            CreateValidOrderItem()
        };

        // Act
        var order = Order.Create(customerEmail, customerName, address, items);

        // Assert
        order.Should().NotBeNull();
        order.CustomerEmail.Should().Be(customerEmail);
        order.CustomerName.Should().Be(customerName);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Amount.Should().BeGreaterThan(0);
        
        // Verifica se o evento foi disparado
        order.DomainEvents.Should().ContainSingle(e => e is OrderCreatedEvent);
    }

    [Fact]
    public void Create_WithEmptyEmail_ShouldThrowException()
    {
        // Arrange
        var customerName = "João Silva";
        var address = CreateValidAddress();
        var items = new List<OrderItem> { CreateValidOrderItem() };

        // Act & Assert
        var act = () => Order.Create("", customerName, address, items);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Customer email is required*");
    }

    [Fact]
    public void Create_WithNoItems_ShouldThrowException()
    {
        // Arrange
        var customerEmail = "test@example.com";
        var customerName = "João Silva";
        var address = CreateValidAddress();
        var items = new List<OrderItem>();

        // Act & Assert
        var act = () => Order.Create(customerEmail, customerName, address, items);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Order must have at least one item*");
    }

    [Fact]
    public void MarkAsPaid_WhenPending_ShouldUpdateStatusAndDispatchEvent()
    {
        // Arrange
        var order = CreateValidOrder();
        var paymentId = "payment_123";

        // Act
        order.MarkAsPaid(paymentId);

        // Assert
        order.Status.Should().Be(OrderStatus.Paid);
        order.PaidAt.Should().NotBeNull();
        order.DomainEvents.Should().Contain(e => e is OrderPaidEvent);
    }

    [Fact]
    public void MarkAsPaid_WhenNotPending_ShouldThrowException()
    {
        // Arrange
        var order = CreateValidOrder();
        order.MarkAsPaid("payment_123"); // Já marca como pago

        // Act & Assert
        var act = () => order.MarkAsPaid("payment_456");
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Cannot mark order as paid*");
    }

    [Fact]
    public void MarkAsShipped_WhenPaid_ShouldUpdateStatusAndDispatchEvent()
    {
        // Arrange
        var order = CreateValidOrder();
        order.MarkAsPaid("payment_123");
        order.ClearDomainEvents(); // Limpa eventos anteriores
        var trackingCode = "BR123456789";

        // Act
        order.MarkAsShipped(trackingCode);

        // Assert
        order.Status.Should().Be(OrderStatus.Shipped);
        order.ShippedAt.Should().NotBeNull();
        order.TrackingCode.Should().Be(trackingCode);
        order.DomainEvents.Should().Contain(e => e is OrderShippedEvent);
    }

    [Fact]
    public void Cancel_WhenPending_ShouldUpdateStatusAndDispatchEvent()
    {
        // Arrange
        var order = CreateValidOrder();
        var reason = "Customer requested cancellation";

        // Act
        order.Cancel(reason);

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.DomainEvents.Should().Contain(e => e is OrderCancelledEvent);
    }

    [Fact]
    public void Cancel_WhenShipped_ShouldThrowException()
    {
        // Arrange
        var order = CreateValidOrder();
        order.MarkAsPaid("payment_123");
        order.MarkAsShipped("BR123456789");

        // Act & Assert
        var act = () => order.Cancel("Too late");
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Cannot cancel order*");
    }

    // Helper methods pra criar objetos válidos nos testes
    private static Order CreateValidOrder()
    {
        return Order.Create(
            "test@example.com",
            "João Silva",
            CreateValidAddress(),
            new List<OrderItem> { CreateValidOrderItem() }
        );
    }

    private static Address CreateValidAddress()
    {
        return new Address(
            "Rua das Flores",
            "123",
            "Centro",
            "São Paulo",
            "SP",
            "01234567"
        );
    }

    private static OrderItem CreateValidOrderItem()
    {
        return OrderItem.Create(
            Guid.NewGuid(),
            "Produto Teste",
            "PROD-001",
            new Money(99.90m),
            2
        );
    }
}