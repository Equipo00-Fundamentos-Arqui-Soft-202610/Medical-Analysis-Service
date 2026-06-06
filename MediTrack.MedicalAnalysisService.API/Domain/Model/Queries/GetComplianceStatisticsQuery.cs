namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

public record GetComplianceStatisticsQuery(string Category, DateTime From, DateTime To);
