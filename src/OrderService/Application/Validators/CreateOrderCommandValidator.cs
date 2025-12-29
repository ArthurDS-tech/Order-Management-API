using FluentValidation;
using OrderService.Application.Commands;

namespace OrderService.Application.Validators;

/// <summary>
/// Validador pro comando de criar pedido - aqui a gente garante que os dados fazem sentido
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        // Email é obrigatório e tem que ser válido
        RuleFor(x => x.CustomerEmail)
            .NotEmpty()
            .WithMessage("Customer email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(255)
            .WithMessage("Email too long");

        // Nome do cliente
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(255)
            .WithMessage("Name too long")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Name can only contain letters and spaces");

        // Endereço de entrega
        RuleFor(x => x.ShippingAddress)
            .NotNull()
            .WithMessage("Shipping address is required")
            .SetValidator(new AddressDtoValidator());

        // Itens do pedido
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must have at least one item")
            .Must(items => items.Count <= 50)
            .WithMessage("Order cannot have more than 50 items"); // Limite razoável

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemDtoValidator());
    }
}

/// <summary>
/// Validador pro endereço
/// </summary>
public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required")
            .MaximumLength(255)
            .WithMessage("Street name too long");

        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Number is required")
            .MaximumLength(20)
            .WithMessage("Number too long");

        RuleFor(x => x.Neighborhood)
            .NotEmpty()
            .WithMessage("Neighborhood is required")
            .MaximumLength(255)
            .WithMessage("Neighborhood name too long");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .MaximumLength(255)
            .WithMessage("City name too long");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required")
            .MaximumLength(100)
            .WithMessage("State name too long");

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("ZIP code is required")
            .Matches(@"^\d{5}-?\d{3}$")
            .WithMessage("Invalid ZIP code format (use 12345-678 or 12345678)");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .MaximumLength(100)
            .WithMessage("Country name too long");

        // Complement é opcional, mas se informado, tem limite
        RuleFor(x => x.Complement)
            .MaximumLength(255)
            .WithMessage("Complement too long")
            .When(x => !string.IsNullOrEmpty(x.Complement));
    }
}

/// <summary>
/// Validador pro item do pedido
/// </summary>
public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(255)
            .WithMessage("Product name too long");

        RuleFor(x => x.ProductSku)
            .NotEmpty()
            .WithMessage("Product SKU is required")
            .MaximumLength(100)
            .WithMessage("SKU too long")
            .Matches(@"^[A-Z0-9-_]+$")
            .WithMessage("SKU can only contain uppercase letters, numbers, hyphens and underscores");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero")
            .LessThan(1000000)
            .WithMessage("Price too high"); // Limite razoável

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero")
            .LessThan(1000)
            .WithMessage("Quantity too high"); // Evita pedidos absurdos
    }
}