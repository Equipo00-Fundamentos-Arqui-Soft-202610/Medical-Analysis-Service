namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Events;

public record AppointmentAttendanceRegisteredIntegrationEvent(
    int PatientId,
    int AppointmentId,
    string AttendanceStatus,
    DateTime OccurredAt);
