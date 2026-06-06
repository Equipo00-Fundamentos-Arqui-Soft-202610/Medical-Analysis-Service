using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IStatisticsQueryService
{
    Task<IEnumerable<ComplianceStatistic>> HandleAsync(GetComplianceStatisticsQuery query);
    Task<IEnumerable<ComplianceStatistic>> HandleAsync(GetAppointmentStatisticsQuery query);
}
