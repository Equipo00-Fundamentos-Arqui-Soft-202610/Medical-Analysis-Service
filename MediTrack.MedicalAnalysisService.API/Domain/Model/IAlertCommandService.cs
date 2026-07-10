using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IAlertCommandService
{
    Task<AdherenceAlert> HandleAsync(RaiseAdherenceAlertCommand command);
    Task<AdherenceAlert> HandleAsync(AcknowledgeAlertCommand command);
    Task<bool> HasOpenAlertAsync(int patientId);
}
