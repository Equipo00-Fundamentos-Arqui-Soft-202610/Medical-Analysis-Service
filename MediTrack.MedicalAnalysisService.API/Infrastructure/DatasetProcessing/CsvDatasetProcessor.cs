using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.DatasetProcessing;

public class CsvDatasetProcessor : IDatasetProcessor
{
    private readonly ILogger<CsvDatasetProcessor> _logger;

    public CsvDatasetProcessor(ILogger<CsvDatasetProcessor> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<ClinicalRecord>> ProcessAsync(Stream dataStream, string importBatchId)
    {
        var records = new List<ClinicalRecord>();

        using var reader = new StreamReader(dataStream);
        var line = await reader.ReadLineAsync();
        var lineNumber = 1;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                var record = ParseLine(line, importBatchId);
                if (record != null)
                    records.Add(record);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Skipping line {Line} in batch {BatchId}: {Error}", lineNumber, importBatchId, ex.Message);
            }
        }

        return records;
    }

    private static ClinicalRecord? ParseLine(string line, string importBatchId)
    {
        var parts = line.Split(',', StringSplitOptions.None);
        if (parts.Length < 3)
            return null;

        if (!int.TryParse(parts[0].Trim(), out var patientId))
            return null;

        if (!DateTime.TryParse(parts[1].Trim(), out var recordDate))
            return null;

        var diagnosis = parts[2].Trim();
        var notes = parts.Length > 3 ? parts[3].Trim() : null;

        return new ClinicalRecord(patientId, recordDate, diagnosis, notes, "dataset", importBatchId);
    }
}
