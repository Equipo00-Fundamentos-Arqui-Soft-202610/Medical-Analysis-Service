using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Events;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.EventHandlers;

public class ComplianceRegisteredEventHandler
{
    private readonly IAdherenceMetricCommandService _metricCommandService;

    public ComplianceRegisteredEventHandler(IAdherenceMetricCommandService metricCommandService)
    {
        _metricCommandService = metricCommandService;
    }

    public async Task HandleAsync(ComplianceRegisteredIntegrationEvent integrationEvent)
    {
        var command = new RecalculateAdherenceMetricCommand(
            integrationEvent.PatientId,
            integrationEvent.Category,
            integrationEvent.WasCompliant);

        await _metricCommandService.HandleAsync(command);
    }
}
