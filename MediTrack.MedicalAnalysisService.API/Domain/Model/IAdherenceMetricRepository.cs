using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IAdherenceMetricRepository
{
    Task<AdherenceMetric> AddAsync(AdherenceMetric metric);
    Task<AdherenceMetric> UpdateAsync(AdherenceMetric metric);
    Task<AdherenceMetric?> FindByPatientIdAndCategoryAsync(int patientId, string category);
    Task<IEnumerable<AdherenceMetric>> FindByPatientIdAsync(int patientId);
}
