namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

public record GetClinicalHistoryByPatientIdQuery(int PatientId, DateTime? From = null, DateTime? To = null);
