using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;
using MediTrack.MedicalAnalysisService.API.Infrastructure.DatasetProcessing;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.CommandServices;

public class ClinicalDataCommandService : IClinicalDataCommandService
{
    private readonly IClinicalRecordRepository _recordRepository;
    private readonly IDatasetProcessor _datasetProcessor;

    public ClinicalDataCommandService(
        IClinicalRecordRepository recordRepository,
        IDatasetProcessor datasetProcessor)
    {
        _recordRepository = recordRepository;
        _datasetProcessor = datasetProcessor;
    }

    public async Task<ClinicalRecord> HandleAsync(RegisterClinicalRecordCommand command)
    {
        var record = new ClinicalRecord(
            command.PatientId,
            command.RecordDate,
            command.Diagnosis,
            command.Notes);

        return await _recordRepository.AddAsync(record);
    }

    public async Task<string> HandleAsync(ImportClinicalDatasetCommand command)
    {
        var records = await _datasetProcessor.ProcessAsync(command.DataStream, command.ImportBatchId);

        foreach (var record in records)
            await _recordRepository.AddAsync(record);

        return command.ImportBatchId;
    }
}
