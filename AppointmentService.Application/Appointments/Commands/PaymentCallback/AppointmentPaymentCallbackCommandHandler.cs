using AppointmentService.Application.Appointments.Commands.InsertAppointment;
using AppointmentService.Application.PaymentServices;
using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.Appointments.Commands.PaymentCallback;

public class AppointmentPaymentCallbackCommandHandler : ICommandHandler<AppointmentPaymentCallbackCommand, AppointmentInsertResponse>
{
    private readonly ICommandRepository<Appointment> _appointmentRepository;
    private readonly INoSqlQueryRepository<AppointmentCollection> _appointmentCollectionRepository;
    private readonly ICommandRepository<Payment> _paymentRepository;
    private readonly INoSqlQueryRepository<PaymentCollection> _paymentCollectionRepository;
    private readonly IIdentityService _identityService;
    private readonly IPaymentService _paymentService;

    public AppointmentPaymentCallbackCommandHandler(ICommandRepository<Appointment> appointmentRepository, IIdentityService identityService, INoSqlQueryRepository<AppointmentCollection> appointmentCollectionRepository, IPaymentService paymentService, ICommandRepository<Payment> paymentRepository, INoSqlQueryRepository<PaymentCollection> paymentCollectionRepository)
    {
        _appointmentRepository = appointmentRepository;
        _identityService = identityService;
        _appointmentCollectionRepository = appointmentCollectionRepository;
        _paymentService = paymentService;
        _paymentRepository = paymentRepository;
        _paymentCollectionRepository = paymentCollectionRepository;
    }

    public async Task<AppointmentInsertResponse> Handle(AppointmentPaymentCallbackCommand request, CancellationToken cancellationToken)
    {
        var response = new AppointmentInsertResponse { Success = false };
        
        // Get current user
        var currentUser = _identityService.GetCurrentUser();
        
        // Get appointment and appointment collection
        var appointment = await _appointmentRepository
            .Find(x => x.AppointmentId == request.AppointmentId && x.IsActive && x.StatusId == ((byte) ConstantEnum.AppointmentStatus.Pending), isTracking: true)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var apppointmentCollection = await _appointmentCollectionRepository
            .FindOneAsync(x => x.Id == request.AppointmentId && x.IsActive && x.Status == ((byte) ConstantEnum.AppointmentStatus.Pending));

        if (appointment == null || apppointmentCollection == null)
        {
            response.SetMessage(MessageId.I00000, "Appointment not found or not available.");
            return response;
        }
        
        // Get payment record
        var payment = await _paymentRepository
            .Find(x => x.AppointmentId == request.AppointmentId && x.IsActive && x.StatusId == ((byte) ConstantEnum.PaymentStatus.Pending), isTracking: true)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        var paymentCollection = await _paymentCollectionRepository
            .FindOneAsync(x => x.AppointmentId == request.AppointmentId && x.IsActive && x.Status == ((byte) ConstantEnum.PaymentStatus.Pending));
        if (payment == null || paymentCollection == null)
        {
            response.SetMessage(MessageId.I00000, "Payment not found or not available.");
            return response;
        }
        
        // Begin transaction
        await _appointmentRepository.ExecuteInTransactionAsync(async () =>
        {
            // Payment failed
            if(request.Code != "00")
            {
                var paymentResponse = await _paymentService.PaymentAppointment(request.AppointmentId);
                if (!paymentResponse.Success)
                {
                    response.SetMessage(MessageId.E00000, "Payment failed.");
                    return false;
                }
                
                // Set response
                response.Success = false;
                response.AppointmentId = request.AppointmentId.ToString();
                response.Response = paymentResponse.Response;
                response.SetMessage(MessageId.I00000, "Payment returned successfully.");
                return false;
            }
            
            // If cancel is true, create new payment record
            if (request.Cancel)
            {
                // Update appointment status to cancelled
                appointment.StatusId = (byte) ConstantEnum.AppointmentStatus.Cancelled;
                apppointmentCollection.Status = (byte) ConstantEnum.AppointmentStatus.Cancelled;
                
                // Update payment status to cancelled
                payment.StatusId = (byte) ConstantEnum.PaymentStatus.Failed;
                paymentCollection.Status = (byte) ConstantEnum.PaymentStatus.Failed;
            }
            else
            {
                // Update appointment status to booked
                appointment.StatusId = (byte) ConstantEnum.AppointmentStatus.Booked;
                apppointmentCollection.Status = (byte) ConstantEnum.AppointmentStatus.Booked;
                
                // Update payment status to success
                payment.StatusId = (byte) ConstantEnum.PaymentStatus.Success;
                paymentCollection.Status = (byte) ConstantEnum.PaymentStatus.Success;
            }
            
            // Save changes
            _appointmentRepository.Update(appointment);
            _paymentRepository.Update(payment);
            await _appointmentRepository.SaveChangesAsync(currentUser.Email);
            
            // Update appointment collection
            _appointmentRepository.Store(apppointmentCollection, currentUser.Email, isModified: true);
            _paymentRepository.Store(paymentCollection, currentUser.Email, isModified: true);
            await _appointmentRepository.SessionSavechanges();
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}