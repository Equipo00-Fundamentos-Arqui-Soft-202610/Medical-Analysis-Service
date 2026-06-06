using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;

public class DashboardResourceFromAggregateAssembler
{
    public AdherenceTrendResource ToResource(int patientId, IEnumerable<AdherenceMetric> metrics)
    {
        var points = metrics.Select(m => new AdherenceTrendPointResource(
            m.Category.Value,
            m.TotalScheduled,
            m.TotalCompliant,
            m.TotalMissed,
            m.Rate.Value,
            m.LastUpdatedAt));

        return new AdherenceTrendResource(patientId, points);
    }
}
