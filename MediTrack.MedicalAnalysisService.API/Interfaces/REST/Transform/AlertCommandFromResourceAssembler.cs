using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;

public class AlertCommandFromResourceAssembler
{
    public RaiseAdherenceAlertCommand ToCommand(RaiseAlertResource resource)
    {
        return new RaiseAdherenceAlertCommand(
            resource.PatientId,
            resource.Severity,
            resource.Reason);
    }
}
