namespace PressPause.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}