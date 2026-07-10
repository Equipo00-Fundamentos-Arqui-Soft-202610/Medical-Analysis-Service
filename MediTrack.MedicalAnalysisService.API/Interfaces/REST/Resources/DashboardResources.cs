namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

public record AdherenceTrendPointResource(
    string Category,
    int TotalScheduled,
    int TotalCompliant,
    int TotalMissed,
    decimal Rate,
    DateTime LastUpdatedAt);

public record AdherenceTrendResource(
    int PatientId,
    IEnumerable<AdherenceTrendPointResource> Points);

public record AllAdherenceMetricResource(
    int PatientId,
    string Category,
    int TotalScheduled,
    int TotalCompliant,
    int TotalMissed,
    decimal Rate,
    DateTime LastUpdatedAt);

public record AllAdherenceMetricsResponse(
    IEnumerable<AllAdherenceMetricResource> Metrics);
