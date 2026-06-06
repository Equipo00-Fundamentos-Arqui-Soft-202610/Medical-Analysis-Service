namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

public record ComplianceStatisticResource(
    int Id,
    string Category,
    DateTime WindowStart,
    DateTime WindowEnd,
    int TotalScheduled,
    int TotalCompliant,
    int TotalMissed,
    decimal ComplianceRate,
    DateTime CalculatedAt);
