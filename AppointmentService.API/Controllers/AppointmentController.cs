using AppointmentService.Application.Appointments.Commands;
using AppointmentService.Application.Appointments.Queries;
using Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace AppointmentService.API.Controllers;

/// <summary>
/// AppointmentController - Controller for managing appointments.
/// </summary>
[ApiController]
[Route("api/v1/appointment")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
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
        var errorList = AbstractFunction<BaseCommandResponse, string>.ErrorCheck(ModelState);
        if (errorList.Count > 0)
        {
            return BadRequest(new BaseCommandResponse
            {
                Success = false,
                DetailErrorList = errorList
            });
        }
        var result = await _mediator.Send(request);
        if (result.Success)
        {
            return Created(result.Response, result);
        }
        
        return BadRequest(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> SelectAppointmentsAsync()
    {
        var result = await _mediator.Send(new AppointmentSelectsQuery());
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
    
    
}