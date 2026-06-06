using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.QueryServices;

public class DashboardQueryService : IDashboardQueryService
{
    private readonly IAdherenceMetricRepository _metricRepository;

    public DashboardQueryService(IAdherenceMetricRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }

    public async Task<IEnumerable<AdherenceMetric>> HandleAsync(GetAdherenceTrendByPatientIdQuery query)
    {
        if (query.PatientId <= 0)
            throw new ArgumentException("PatientId must be greater than 0", nameof(query.PatientId));

        return await _metricRepository.FindByPatientIdAsync(query.PatientId);
    }
}
