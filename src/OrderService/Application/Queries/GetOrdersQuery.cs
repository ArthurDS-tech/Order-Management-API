using SharedKernel.Application;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Queries;

/// <summary>
/// Query pra listar pedidos com filtros e paginação - essencial pra não quebrar o sistema
/// </summary>
public class GetOrdersQuery : IQuery<PagedResult<OrderSummaryDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CustomerEmail { get; set; }
    public OrderStatus? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SortBy { get; set; } = "CreatedAt"; // CreatedAt, TotalAmount, Status
    public bool SortDescending { get; set; } = true;
}

/// <summary>
/// DTO resumido do pedido - pra listagens não precisamos de todos os detalhes
/// </summary>
public class OrderSummaryDto
{
    public Guid Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? TrackingCode { get; set; }
}

/// <summary>
/// Resultado paginado - padrão pra não sobrecarregar o frontend
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}