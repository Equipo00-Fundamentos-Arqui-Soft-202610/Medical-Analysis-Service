namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

public record GetAdherenceTrendByPatientIdQuery(int PatientId, DateTime From, DateTime To);
