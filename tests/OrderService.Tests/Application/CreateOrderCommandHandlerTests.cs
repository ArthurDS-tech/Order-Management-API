using FluentAssertions;
using Moq;
using MediatR;
using OrderService.Application.Commands;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Tests.Application;

/// <summary>
/// Testes do handler de criar pedido - aqui testamos a lógica de aplicação
/// </summary>
public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IMediator> _mockMediator;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockMediator = new Mock<IMediator>();
        _handler = new CreateOrderCommandHandler(_mockRepository.Object, _mockMediator.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateOrderAndReturnId()
    {
        // Arrange
        var command = CreateValidCommand();
        
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Order>()))
                      .Returns(Task.CompletedTask);
        
        _mockRepository.Setup(r => r.SaveChangesAsync())
                      .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        // Verifica se o repositório foi chamado
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        
        // Verifica se os eventos foram publicados
        _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), 
                           Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CustomerEmail = ""; // Email inválido

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid data");
        
        // Não deve chamar o repositório
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldReturnFailure()
    {
        // Arrange
        var command = CreateValidCommand();
        
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Order>()))
                      .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Failed to create order");
    }

    private static CreateOrderCommand CreateValidCommand()
    {
        return new CreateOrderCommand
        {
            CustomerEmail = "test@example.com",
            CustomerName = "João Silva",
            ShippingAddress = new AddressDto
            {
                Street = "Rua das Flores",
                Number = "123",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                ZipCode = "01234567",
                Country = "Brasil"
            },
            Items = new List<OrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Teste",
                    ProductSku = "PROD-001",
                    Price = 99.90m,
                    Quantity = 2
                }
            }
        };
    }
}