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

        var metrics = await _metricRepository.FindByPatientIdAsync(query.PatientId);

        // LIMITACIÓN CONOCIDA: AdherenceMetric guarda un único snapshot acumulado
        // por paciente+categoría (no una serie histórica día a día), así que
        // "From"/"To" solo puede filtrar por LastUpdatedAt -- no arma una
        // verdadera tendencia por rango. Un trend real requiere un modelo de
        // series de tiempo por paciente (fuera del alcance de este fix).
        return metrics.Where(m => m.LastUpdatedAt >= query.From && m.LastUpdatedAt <= query.To);
    }

    public async Task<IEnumerable<AdherenceMetric>> HandleAsync(GetAllAdherenceMetricsQuery query)
    {
        return await _metricRepository.FindAllAsync();
    }
}
