using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;

public class AlertResourceFromEntityAssembler
{
    public AlertResource ToResource(AdherenceAlert alert)
    {
        return new AlertResource(
            alert.Id,
            alert.PatientId,
            alert.Severity.Value,
            alert.Status.Value,
            alert.Reason,
            alert.TriggeredAt,
            alert.AcknowledgedAt);
    }

    public IEnumerable<AlertResource> ToResources(IEnumerable<AdherenceAlert> alerts) =>
        alerts.Select(ToResource);
}
