using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IAlertQueryService
{
    Task<IEnumerable<AdherenceAlert>> HandleAsync(GetActiveAlertsQuery query);
}
