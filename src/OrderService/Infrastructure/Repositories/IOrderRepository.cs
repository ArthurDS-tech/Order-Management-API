using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using OrderService.Application.Queries;

namespace OrderService.Infrastructure.Repositories;

/// <summary>
/// Interface do repositório - define o que podemos fazer com pedidos
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order?> GetByIdWithItemsAsync(Guid id);
    Task<PagedResult<Order>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? customerEmail = null,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string sortBy = "CreatedAt",
        bool sortDescending = true);
    
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Order order);
    Task SaveChangesAsync();
    
    // Métodos específicos pra relatórios e dashboards
    Task<int> GetTotalOrdersCountAsync();
    Task<decimal> GetTotalRevenueAsync();
    Task<List<Order>> GetRecentOrdersAsync(int count = 10);
}