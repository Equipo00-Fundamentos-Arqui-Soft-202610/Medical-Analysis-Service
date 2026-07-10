using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IDashboardQueryService
{
    Task<IEnumerable<AdherenceMetric>> HandleAsync(GetAdherenceTrendByPatientIdQuery query);
    Task<IEnumerable<AdherenceMetric>> HandleAsync(GetAllAdherenceMetricsQuery query);
}
