using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

public class InMemoryBus : IBus
{
    public List<DomainEvent> Events { get; } = new List<DomainEvent>();

    public Task PublishAsync(DomainEvent domainEvent)
    {
        Events.Add(domainEvent);
        return Task.CompletedTask;
    }
}
