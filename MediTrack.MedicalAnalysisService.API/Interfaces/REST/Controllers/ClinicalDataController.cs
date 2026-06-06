using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/clinical-records")]
public class ClinicalDataController : ControllerBase
{
    private readonly IClinicalDataCommandService _commandService;
    private readonly IClinicalDataQueryService _queryService;
    private readonly ClinicalDataCommandFromResourceAssembler _commandAssembler;
    private readonly ClinicalRecordResourceFromEntityAssembler _resourceAssembler;

    public ClinicalDataController(
        IClinicalDataCommandService commandService,
        IClinicalDataQueryService queryService,
        ClinicalDataCommandFromResourceAssembler commandAssembler,
        ClinicalRecordResourceFromEntityAssembler resourceAssembler)
    {
        _commandService = commandService;
        _queryService = queryService;
        _commandAssembler = commandAssembler;
        _resourceAssembler = resourceAssembler;
    }

    [HttpPost]
    public async Task<ActionResult<ClinicalRecordResource>> CreateClinicalRecord(
        [FromBody] CreateClinicalRecordResource resource)
    {
        try
        {
            var command = _commandAssembler.ToCommand(resource);
            var record = await _commandService.HandleAsync(command);
            return CreatedAtAction(nameof(GetClinicalHistory),
                new { patientId = record.PatientId },
                _resourceAssembler.ToResource(record));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("import")]
    public async Task<ActionResult<ImportClinicalDatasetResource>> ImportDataset(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            var importBatchId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var command = new ImportClinicalDatasetCommand(file.OpenReadStream(), importBatchId);
            await _commandService.HandleAsync(command);

            return Accepted(new ImportClinicalDatasetResource(importBatchId));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClinicalRecordResource>>> GetClinicalHistory(
        [FromQuery] int patientId)
    {
        try
        {
            var records = await _queryService.HandleAsync(new GetClinicalHistoryByPatientIdQuery(patientId));
            if (!records.Any())
                return NotFound(new { message = $"No clinical records found for patient {patientId}" });

            return Ok(_resourceAssembler.ToResources(records));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
