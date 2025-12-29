using SharedKernel.Domain;

namespace PaymentService.Domain.Entities;

/// <summary>
/// Entidade Payment - representa um pagamento no sistema
/// </summary>
public class Payment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public string CustomerEmail { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "BRL";
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string PaymentMethod { get; private set; } = string.Empty; // Credit Card, PIX, etc.
    public string? ExternalPaymentId { get; private set; } // ID do gateway (Stripe, etc.)
    public string? FailureReason { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    // EF Core constructor
    private Payment() { }

    public static Payment Create(Guid orderId, string customerEmail, decimal amount, string paymentMethod)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty", nameof(orderId));
        
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email is required", nameof(customerEmail));
        
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        return new Payment
        {
            OrderId = orderId,
            CustomerEmail = customerEmail.ToLowerInvariant(),
            Amount = amount,
            PaymentMethod = paymentMethod,
            Status = PaymentStatus.Pending
        };
    }

    public void MarkAsProcessing(string externalPaymentId)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot mark as processing. Current status: {Status}");

        Status = PaymentStatus.Processing;
        ExternalPaymentId = externalPaymentId;
        MarkAsUpdated();
    }

    public void MarkAsCompleted()
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException($"Cannot complete payment. Current status: {Status}");

        Status = PaymentStatus.Completed;
        ProcessedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void MarkAsFailed(string reason)
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Cannot fail a completed payment");

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        MarkAsUpdated();
    }
}

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Refunded = 5
}