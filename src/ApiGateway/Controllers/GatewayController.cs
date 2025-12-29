using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

/// <summary>
/// Controller do Gateway - endpoints de controle e informações
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    [HttpGet("info")]
    public ActionResult GetInfo()
    {
        return Ok(new
        {
            service = "API Gateway",
            version = "1.0.0",
            timestamp = DateTime.UtcNow,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            availableServices = new[]
            {
                "orders - /api/orders",
                "payments - /api/payments", 
                "inventory - /api/inventory",
                "notifications - /api/notifications"
            }
        });
    }

    [HttpGet("health")]
    public ActionResult Health()
    {
        return Ok(new { 
            status = "healthy", 
            service = "gateway", 
            timestamp = DateTime.UtcNow 
        });
    }
}