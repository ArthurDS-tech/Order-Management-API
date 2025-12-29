using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using OrderService.Infrastructure.Data.Configurations;

namespace OrderService.Infrastructure.Data;

/// <summary>
/// DbContext do Order Service - configurações do EF Core
/// </summary>
public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica as configurações - melhor separar pra não virar bagunça
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configurações extras pra desenvolvimento
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
    }

    // Override pra adicionar auditoria automática
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Aqui a gente pode adicionar auditoria automática, logs, etc.
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // CreatedAt já é setado no construtor da BaseEntity
                    break;
                case EntityState.Modified:
                    entry.Entity.GetType()
                        .GetProperty("UpdatedAt")?
                        .SetValue(entry.Entity, DateTime.UtcNow);
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}