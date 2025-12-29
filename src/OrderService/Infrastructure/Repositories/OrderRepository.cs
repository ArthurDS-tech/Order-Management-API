using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using OrderService.Infrastructure.Data;
using OrderService.Application.Queries;

namespace OrderService.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de pedidos - aqui é onde batemos no banco
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetByIdWithItemsAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<PagedResult<Order>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? customerEmail = null,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string sortBy = "CreatedAt",
        bool sortDescending = true)
    {
        var query = _context.Orders.Include(o => o.Items).AsQueryable();

        // Aplica filtros - só se foram informados
        if (!string.IsNullOrWhiteSpace(customerEmail))
        {
            query = query.Where(o => o.CustomerEmail.Contains(customerEmail.ToLowerInvariant()));
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= endDate.Value);
        }

        // Aplica ordenação - switch expression é mais limpo que if/else
        query = sortBy.ToLowerInvariant() switch
        {
            "createdat" => sortDescending 
                ? query.OrderByDescending(o => o.CreatedAt)
                : query.OrderBy(o => o.CreatedAt),
            "totalamount" => sortDescending
                ? query.OrderByDescending(o => o.TotalAmount.Amount)
                : query.OrderBy(o => o.TotalAmount.Amount),
            "status" => sortDescending
                ? query.OrderByDescending(o => o.Status)
                : query.OrderBy(o => o.Status),
            _ => query.OrderByDescending(o => o.CreatedAt) // Default
        };

        var totalCount = await query.CountAsync();
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Order>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    // Métodos específicos pra dashboards e relatórios
    public async Task<int> GetTotalOrdersCountAsync()
    {
        return await _context.Orders.CountAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .SumAsync(o => o.TotalAmount.Amount);
    }

    public async Task<List<Order>> GetRecentOrdersAsync(int count = 10)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}