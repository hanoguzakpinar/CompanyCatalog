namespace CompanyCatalog.Domain.Common;

public interface IDomainEvent
{
    public DateTime OccuredOn { get; }
}