using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;

public class ClinicalRecordResourceFromEntityAssembler
{
    public ClinicalRecordResource ToResource(ClinicalRecord record)
    {
        return new ClinicalRecordResource(
            record.Id,
            record.PatientId,
            record.RecordDate,
            record.Diagnosis,
            record.Notes,
            record.Source,
            record.ImportBatchId,
            record.CreatedAt);
    }

    public IEnumerable<ClinicalRecordResource> ToResources(IEnumerable<ClinicalRecord> records) =>
        records.Select(ToResource);
}
