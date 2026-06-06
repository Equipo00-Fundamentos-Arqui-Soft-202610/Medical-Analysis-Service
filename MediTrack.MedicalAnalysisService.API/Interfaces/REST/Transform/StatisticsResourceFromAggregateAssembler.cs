using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;

public class StatisticsResourceFromAggregateAssembler
{
    public ComplianceStatisticResource ToResource(ComplianceStatistic statistic)
    {
        return new ComplianceStatisticResource(
            statistic.Id,
            statistic.Category.Value,
            statistic.WindowStart,
            statistic.WindowEnd,
            statistic.TotalScheduled,
            statistic.TotalCompliant,
            statistic.TotalMissed,
            statistic.ComplianceRate.Value,
            statistic.CalculatedAt);
    }

    public IEnumerable<ComplianceStatisticResource> ToResources(IEnumerable<ComplianceStatistic> statistics) =>
        statistics.Select(ToResource);
}
