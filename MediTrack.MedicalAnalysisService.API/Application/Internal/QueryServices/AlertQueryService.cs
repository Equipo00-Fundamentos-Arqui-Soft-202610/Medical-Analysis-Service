using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.QueryServices;

public class AlertQueryService : IAlertQueryService
{
    private readonly IAdherenceAlertRepository _alertRepository;

    public AlertQueryService(IAdherenceAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<IEnumerable<AdherenceAlert>> HandleAsync(GetActiveAlertsQuery query)
    {
        return await _alertRepository.FindByStatusAsync(query.Status);
    }
}
