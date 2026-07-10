using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/alerts")]
public class AlertController : ControllerBase
{
    private readonly IAlertCommandService _commandService;
    private readonly IAlertQueryService _queryService;
    private readonly AlertCommandFromResourceAssembler _commandAssembler;
    private readonly AlertResourceFromEntityAssembler _resourceAssembler;

    public AlertController(
        IAlertCommandService commandService,
        IAlertQueryService queryService,
        AlertCommandFromResourceAssembler commandAssembler,
        AlertResourceFromEntityAssembler resourceAssembler)
    {
        _commandService = commandService;
        _queryService = queryService;
        _commandAssembler = commandAssembler;
        _resourceAssembler = resourceAssembler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlertResource>>> GetAlerts([FromQuery] string? status)
    {
        try
        {
            var alerts = await _queryService.HandleAsync(new GetActiveAlertsQuery(status));
            if (!alerts.Any())
                return NotFound(new { message = "No alerts found" });

            return Ok(_resourceAssembler.ToResources(alerts));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<AlertResource>> RaiseAlert([FromBody] RaiseAlertResource resource)
    {
        try
        {
            var command = _commandAssembler.ToCommand(resource);
            var alert = await _commandService.HandleAsync(command);
            return CreatedAtAction(nameof(GetAlerts), new { status = "open" },
                _resourceAssembler.ToResource(alert));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/acknowledge")]
    public async Task<ActionResult<AlertResource>> AcknowledgeAlert(int id)
    {
        try
        {
            var alert = await _commandService.HandleAsync(new AcknowledgeAlertCommand(id));
            return Ok(_resourceAssembler.ToResource(alert));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
