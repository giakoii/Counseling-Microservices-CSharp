using AppointmentService.Application.Appointments.Commands;
using AppointmentService.Application.Appointments.Queries;
using Common;
using Common.Utils.Const;
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
    
    /// <summary>
    /// Insert a new appointment.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> InsertAppointment([FromBody] AppointmentInsertCommand request)
    {
        var response = new AppointmentInsertResponse { Success = false };
        var errorList = AbstractFunction<AppointmentInsertResponse, AppointmentInsertEntity>.ErrorCheck(ModelState);
        if (errorList.Count > 0)
        {
            response.SetMessage(MessageId.E10000);
            return BadRequest(response);
        }
        response = await _mediator.Send(request);
        if (response.Success)
        {
            return Created(response.AppointmentId, response);
        }
        
        return BadRequest(response);
    }
    
    /// <summary>
    /// Select all appointments.
    /// </summary>
    /// <returns></returns>
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
    
    /// <summary>
    /// Update the status of an appointment.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateAppointmentStatusAsync([FromRoute] Guid id, [FromBody] AppointmentUpdateStatusCommand request)
    {
        var response = new BaseCommandResponse { Success = false };
        request.AppointmentId = id;

        var errorList = AbstractFunction<BaseCommandResponse, string>.ErrorCheck(ModelState);
        if (errorList.Count > 0)
        {
            response.SetMessage(MessageId.E10000);
            return BadRequest(response);
        }
        response = await _mediator.Send(request);
        if (response.Success)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpPatch("{appointmentId}/payment-call-back")]
    public async Task<IActionResult> AppointmentPaymentCallbackAsync([FromRoute] Guid appointmentId, [FromBody] AppointmentPaymentCallbackCommand request)
    {
        var response = new AppointmentInsertResponse { Success = false };
        request.AppointmentId = appointmentId;

        var errorList = AbstractFunction<AppointmentInsertResponse, AppointmentInsertEntity>.ErrorCheck(ModelState);
        if (errorList.Count > 0)
        {
            response.SetMessage(MessageId.E10000);
            return BadRequest(response);
        }
        response = await _mediator.Send(request);
        if (response.Success)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
}