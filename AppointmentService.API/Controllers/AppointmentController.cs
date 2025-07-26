using AppointmentService.Application.Appointments.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentService.API.Controllers;

/// <summary>
/// AppointmentController - Controller for managing appointments.
/// </summary>
[ApiController]
[Route("api/v1/appointment")]
public class AppointmentController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mediator"></param>
    public AppointmentController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> InsertAppointment([FromBody] AppointmentInsertCommand request)
    {
        var result = await _mediator.Send(request);
        if (result.Success)
        {
            return CreatedAtAction(result.Response, result);
        }
        
        return Ok(new { Message = "This endpoint will return appointments." });
    }
    
}