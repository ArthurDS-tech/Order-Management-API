using MediatR;

namespace SharedKernel.Domain;

/// <summary>
/// Interface base pra eventos de domínio - quando algo importante rola no sistema
/// </summary>
public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}

/// <summary>
/// Base class pra facilitar a criação de eventos
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}