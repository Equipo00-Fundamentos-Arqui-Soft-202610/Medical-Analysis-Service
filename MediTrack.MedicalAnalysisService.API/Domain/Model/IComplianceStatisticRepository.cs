using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IComplianceStatisticRepository
{
    Task<ComplianceStatistic> AddAsync(ComplianceStatistic statistic);
    Task<ComplianceStatistic> UpdateAsync(ComplianceStatistic statistic);
    Task<IEnumerable<ComplianceStatistic>> FindByCategoryAndRangeAsync(string category, DateTime from, DateTime to);
    Task<IEnumerable<ComplianceStatistic>> FindByRangeAsync(DateTime from, DateTime to);
}
