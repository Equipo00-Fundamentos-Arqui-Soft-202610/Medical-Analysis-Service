using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;

public class ClinicalDataCommandFromResourceAssembler
{
    public RegisterClinicalRecordCommand ToCommand(CreateClinicalRecordResource resource)
    {
        return new RegisterClinicalRecordCommand(
            resource.PatientId,
            resource.RecordDate,
            resource.Diagnosis,
            resource.Notes);
    }
}
