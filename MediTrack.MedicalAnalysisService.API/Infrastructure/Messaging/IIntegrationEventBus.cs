namespace MediTrack.MedicalAnalysisService.API.Infrastructure.Messaging;

public interface IIntegrationEventBus
{
    void Publish<TEvent>(TEvent integrationEvent) where TEvent : class;
    void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
}
