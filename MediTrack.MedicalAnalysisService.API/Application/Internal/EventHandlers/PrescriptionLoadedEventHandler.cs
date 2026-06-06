using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Events;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.EventHandlers;

public class PrescriptionLoadedEventHandler
{
    private readonly IAdherenceMetricRepository _metricRepository;

    public PrescriptionLoadedEventHandler(IAdherenceMetricRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }

    public async Task HandleAsync(PrescriptionLoadedIntegrationEvent integrationEvent)
    {
        var existing = await _metricRepository
            .FindByPatientIdAndCategoryAsync(integrationEvent.PatientId, "medication");

        if (existing == null)
        {
            var metric = new AdherenceMetric(integrationEvent.PatientId, "medication");
            await _metricRepository.AddAsync(metric);
        }
    }
}
