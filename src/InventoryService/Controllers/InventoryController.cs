using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

/// <summary>
/// Controller do Inventory Service - gerencia estoque dos produtos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    [HttpGet("{productId:guid}")]
    public async Task<ActionResult<InventoryDto>> GetInventory(Guid productId)
    {
        // TODO: Buscar no banco real
        await Task.Delay(100);
        
        return Ok(new InventoryDto
        {
            ProductId = productId,
            AvailableQuantity = Random.Shared.Next(0, 100),
            ReservedQuantity = Random.Shared.Next(0, 10),
            LastUpdated = DateTime.UtcNow
        });
    }

    [HttpPost("{productId:guid}/reserve")]
    public async Task<ActionResult> ReserveStock(Guid productId, [FromBody] ReserveStockRequest request)
    {
        // TODO: Implementar lógica de reserva
        await Task.Delay(200);
        
        // Simula sucesso na reserva
        return Ok(new { reservationId = Guid.NewGuid(), message = "Stock reserved successfully" });
    }

    [HttpPost("{productId:guid}/release")]
    public async Task<ActionResult> ReleaseStock(Guid productId, [FromBody] ReleaseStockRequest request)
    {
        // TODO: Implementar lógica de liberação
        await Task.Delay(100);
        
        return Ok(new { message = "Stock released successfully" });
    }

    [HttpGet("health")]
    public ActionResult Health()
    {
        return Ok(new { status = "healthy", service = "inventory", timestamp = DateTime.UtcNow });
    }
}

public class InventoryDto
{
    public Guid ProductId { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class ReserveStockRequest
{
    public int Quantity { get; set; }
    public Guid OrderId { get; set; }
}

public class ReleaseStockRequest
{
    public int Quantity { get; set; }
    public Guid OrderId { get; set; }
}