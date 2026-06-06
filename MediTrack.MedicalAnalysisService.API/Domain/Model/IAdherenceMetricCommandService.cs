using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IAdherenceMetricCommandService
{
    Task<AdherenceMetric> HandleAsync(RecalculateAdherenceMetricCommand command);
}
