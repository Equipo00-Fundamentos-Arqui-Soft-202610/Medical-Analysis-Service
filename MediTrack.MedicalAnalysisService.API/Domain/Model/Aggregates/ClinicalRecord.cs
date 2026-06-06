namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;

public class ClinicalRecord
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime RecordDate { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Source { get; set; } = "manual";
    public string? ImportBatchId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ClinicalRecord() { }

    public ClinicalRecord(int patientId, DateTime recordDate, string diagnosis, string? notes = null,
        string source = "manual", string? importBatchId = null)
    {
        if (patientId <= 0)
            throw new ArgumentException("PatientId must be greater than 0", nameof(patientId));
        if (string.IsNullOrWhiteSpace(diagnosis))
            throw new ArgumentException("Diagnosis cannot be empty", nameof(diagnosis));
        if (source != "manual" && source != "dataset")
            throw new ArgumentException("Source must be manual or dataset", nameof(source));

        PatientId = patientId;
        RecordDate = recordDate;
        Diagnosis = diagnosis.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        Source = source;
        ImportBatchId = importBatchId;
        CreatedAt = DateTime.UtcNow;
    }
}
