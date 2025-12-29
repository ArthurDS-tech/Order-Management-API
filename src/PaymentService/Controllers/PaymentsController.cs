using Microsoft.AspNetCore.Mvc;
using PaymentService.Domain.Entities;

namespace PaymentService.Controllers;

/// <summary>
/// Controller básico do Payment Service - endpoints essenciais
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        // TODO: Implementar lógica de pagamento
        // Por enquanto, só um mock que sempre aprova
        
        var payment = Payment.Create(
            request.OrderId,
            request.CustomerEmail,
            request.Amount,
            request.PaymentMethod
        );

        // Simula processamento com gateway externo
        await Task.Delay(1000); // Simula latência da rede
        
        payment.MarkAsProcessing($"stripe_{Guid.NewGuid()}");
        payment.MarkAsCompleted();

        return Ok(payment.Id);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(Guid id)
    {
        // TODO: Buscar no repositório
        await Task.Delay(100);
        
        return Ok(new PaymentDto
        {
            Id = id,
            Status = "Completed",
            Amount = 199.90m,
            ProcessedAt = DateTime.UtcNow
        });
    }

    [HttpGet("health")]
    public ActionResult Health()
    {
        return Ok(new { status = "healthy", service = "payment", timestamp = DateTime.UtcNow });
    }
}

public class ProcessPaymentRequest
{
    public Guid OrderId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? ProcessedAt { get; set; }
}