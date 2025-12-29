using Microsoft.AspNetCore.Mvc;
using MediatR;
using OrderService.Application.Commands;
using OrderService.Application.Queries;
using OrderService.Domain.ValueObjects;

namespace OrderService.Controllers;

/// <summary>
/// Controller dos pedidos - endpoints REST pra gerenciar pedidos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista pedidos com filtros e paginação
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderSummaryDto>>> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? customerEmail = null,
        [FromQuery] OrderStatus? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = true)
    {
        // Validação básica - não deixa o cliente quebrar o sistema
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20; // Máximo de 100 pra não sobrecarregar

        var query = new GetOrdersQuery
        {
            Page = page,
            PageSize = pageSize,
            CustomerEmail = customerEmail,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Busca um pedido específico por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        if (result.Value == null)
            return NotFound($"Order {id} not found");

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria um novo pedido
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        // Retorna 201 Created com a URL do recurso criado
        return CreatedAtAction(
            nameof(GetOrder), 
            new { id = result.Value }, 
            result.Value);
    }

    /// <summary>
    /// Atualiza status do pedido - usado pelos outros serviços
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult> UpdateOrderStatus(
        Guid id, 
        [FromBody] UpdateOrderStatusRequest request)
    {
        var command = new UpdateOrderStatusCommand
        {
            OrderId = id,
            NewStatus = request.Status,
            TrackingCode = request.TrackingCode,
            PaymentId = request.PaymentId,
            CancellationReason = request.CancellationReason
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent(); // 204 - operação bem sucedida, sem conteúdo pra retornar
    }

    /// <summary>
    /// Endpoint pra health check - útil pro load balancer
    /// </summary>
    [HttpGet("health")]
    public ActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}

/// <summary>
/// Request pra atualizar status - separado do command pra não vazar detalhes internos
/// </summary>
public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
    public string? TrackingCode { get; set; }
    public string? PaymentId { get; set; }
    public string? CancellationReason { get; set; }
}