using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Controllers;

/// <summary>
/// Controller do Notification Service - manda emails, SMS, push notifications
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    [HttpPost("email")]
    public async Task<ActionResult> SendEmail([FromBody] SendEmailRequest request)
    {
        // TODO: Integrar com SendGrid, AWS SES, etc.
        // Por enquanto, só loga que enviou
        
        await Task.Delay(500); // Simula latência do provedor
        
        Console.WriteLine($"📧 Email sent to {request.To}: {request.Subject}");
        
        return Ok(new { 
            messageId = Guid.NewGuid(), 
            status = "sent",
            message = "Email sent successfully" 
        });
    }

    [HttpPost("sms")]
    public async Task<ActionResult> SendSms([FromBody] SendSmsRequest request)
    {
        // TODO: Integrar com Twilio, AWS SNS, etc.
        await Task.Delay(300);
        
        Console.WriteLine($"📱 SMS sent to {request.PhoneNumber}: {request.Message}");
        
        return Ok(new { 
            messageId = Guid.NewGuid(), 
            status = "sent",
            message = "SMS sent successfully" 
        });
    }

    [HttpGet("health")]
    public ActionResult Health()
    {
        return Ok(new { status = "healthy", service = "notification", timestamp = DateTime.UtcNow });
    }
}

public class SendEmailRequest
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
}

public class SendSmsRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}