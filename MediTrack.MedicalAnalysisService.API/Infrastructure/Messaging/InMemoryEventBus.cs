using System.Collections.Concurrent;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.Messaging;

public class InMemoryEventBus : IIntegrationEventBus
{
    private readonly ConcurrentDictionary<Type, List<Func<object, Task>>> _handlers = new();

    public void Publish<TEvent>(TEvent integrationEvent) where TEvent : class
    {
        var eventType = typeof(TEvent);
        if (!_handlers.TryGetValue(eventType, out var handlers))
            return;

        foreach (var handler in handlers)
            _ = handler(integrationEvent);
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);
        _handlers.AddOrUpdate(
            eventType,
            _ => [async e => await handler((TEvent)e)],
            (_, existing) =>
            {
                existing.Add(async e => await handler((TEvent)e));
                return existing;
            });
    }
}
