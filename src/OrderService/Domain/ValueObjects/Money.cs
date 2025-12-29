using SharedKernel.Domain;

namespace OrderService.Domain.ValueObjects;

/// <summary>
/// Value Object pra dinheiro - evita bugs de precisão e facilita operações
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "BRL")
    {
        // Validações básicas - dinheiro negativo só em casos muito específicos
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required", nameof(currency));

        // Arredonda pra 2 casas decimais - padrão pra moeda
        Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero => new(0);

    // Operators pra facilitar cálculos
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract different currencies");
        
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }

    // Comparações
    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot compare different currencies");
        
        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot compare different currencies");
        
        return left.Amount < right.Amount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:C} {Currency}";

    // Método útil pra formatação em relatórios
    public string ToFormattedString() => Currency switch
    {
        "BRL" => Amount.ToString("C", new System.Globalization.CultureInfo("pt-BR")),
        "USD" => Amount.ToString("C", new System.Globalization.CultureInfo("en-US")),
        _ => $"{Amount:F2} {Currency}"
    };
}