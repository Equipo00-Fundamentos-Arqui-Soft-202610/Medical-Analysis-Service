using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Events;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.EventHandlers;

public class AppointmentAttendanceRegisteredEventHandler
{
    private readonly IAdherenceMetricCommandService _metricCommandService;

    public AppointmentAttendanceRegisteredEventHandler(IAdherenceMetricCommandService metricCommandService)
    {
        _metricCommandService = metricCommandService;
    }

    public async Task HandleAsync(AppointmentAttendanceRegisteredIntegrationEvent integrationEvent)
    {
        var wasCompliant = integrationEvent.AttendanceStatus.ToLowerInvariant() == "attended";

        var command = new RecalculateAdherenceMetricCommand(
            integrationEvent.PatientId,
            "appointment",
            wasCompliant);

        await _metricCommandService.HandleAsync(command);
    }
}
