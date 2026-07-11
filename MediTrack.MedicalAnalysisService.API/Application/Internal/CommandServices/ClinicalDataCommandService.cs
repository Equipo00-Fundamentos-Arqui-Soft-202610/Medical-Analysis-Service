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
        var mainRecord = new ClinicalRecord(
            command.PatientId,
            command.RecordDate,
            command.Diagnosis,
            command.Notes,
            "dataset",
            command.ImportBatchId);
        await _recordRepository.AddAsync(mainRecord);

        var memoryStream = new MemoryStream();
        await command.DataStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var records = await _datasetProcessor.ProcessAsync(memoryStream, command.ImportBatchId);
        foreach (var record in records)
            await _recordRepository.AddAsync(record);

        return command.ImportBatchId;
    }
}
