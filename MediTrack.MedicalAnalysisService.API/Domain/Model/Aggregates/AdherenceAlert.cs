using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;

public class AdherenceAlert
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public AlertSeverity Severity { get; set; } = null!;
    public AlertStatus Status { get; set; } = AlertStatus.Open;
    public string Reason { get; set; } = string.Empty;
    public DateTime TriggeredAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }

    public AdherenceAlert() { }

    public AdherenceAlert(int patientId, string severity, string reason)
    {
        if (patientId <= 0)
            throw new ArgumentException("PatientId must be greater than 0", nameof(patientId));
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason cannot be empty", nameof(reason));

        PatientId = patientId;
        Severity = AlertSeverity.From(severity);
        Status = AlertStatus.Open;
        Reason = reason.Trim();
        TriggeredAt = DateTime.UtcNow;
    }

    public void Acknowledge()
    {
        if (!Status.IsOpen)
            throw new ArgumentException("Only open alerts can be acknowledged");

        Status = AlertStatus.Acknowledged;
        AcknowledgedAt = DateTime.UtcNow;
    }

    public void Resolve()
    {
        if (Status.Equals(AlertStatus.Resolved))
            throw new ArgumentException("Alert is already resolved");

        Status = AlertStatus.Resolved;
    }
}
