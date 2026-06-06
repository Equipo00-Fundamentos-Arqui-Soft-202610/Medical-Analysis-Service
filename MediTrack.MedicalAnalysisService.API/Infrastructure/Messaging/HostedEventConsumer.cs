using MediTrack.MedicalAnalysisService.API.Application.Internal.EventHandlers;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Events;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.Messaging;

public class HostedEventConsumer : BackgroundService
{
    private readonly IIntegrationEventBus _eventBus;
    private readonly IServiceProvider _serviceProvider;

    public HostedEventConsumer(IIntegrationEventBus eventBus, IServiceProvider serviceProvider)
    {
        _eventBus = eventBus;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _eventBus.Subscribe<ComplianceRegisteredIntegrationEvent>(async e =>
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<ComplianceRegisteredEventHandler>();
            await handler.HandleAsync(e);
        });

        _eventBus.Subscribe<AppointmentAttendanceRegisteredIntegrationEvent>(async e =>
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<AppointmentAttendanceRegisteredEventHandler>();
            await handler.HandleAsync(e);
        });

        _eventBus.Subscribe<PrescriptionLoadedIntegrationEvent>(async e =>
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<PrescriptionLoadedEventHandler>();
            await handler.HandleAsync(e);
        });

        return Task.CompletedTask;
    }
}
