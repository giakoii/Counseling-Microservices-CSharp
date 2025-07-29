using AppointmentService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.Appointments.Queries.AppointmentSelectIn6Months;



/// <summary>
/// AppointmentSelectIn6Months - Handles the retrieval of appointment counts for the last 6 months and today.
/// </summary>
public class AppointmentSelectIn6MonthsQueryHandler : IQueryHandler<AppointmentSelectIn6MonthsQuery, AppointmentSelectIn6MonthsQueryResponse>
{
    private readonly INoSqlQueryRepository<AppointmentCollection> _appointmentRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appointmentRepository"></param>
    public AppointmentSelectIn6MonthsQueryHandler(INoSqlQueryRepository<AppointmentCollection> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }
    
    public async Task<AppointmentSelectIn6MonthsQueryResponse> Handle(AppointmentSelectIn6MonthsQuery request, CancellationToken cancellationToken)
    {
        var response = new AppointmentSelectIn6MonthsQueryResponse { Success = false };
        
        try
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            
            // Calculate the start date (first day of the month 5 months ago)
            var startDate = new DateOnly(today.Year, today.Month, 1).AddMonths(-5);
            
            // Get all appointments from start date to end of current month
            var endOfCurrentMonth = new DateOnly(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            
            // Get all active appointments in the date range
            var appointments = await _appointmentRepository.FindAllAsync(x => 
                x.IsActive && 
                x.AppointmentDate >= startDate && 
                x.AppointmentDate <= endOfCurrentMonth);
            
            var monthlyData = new List<MonthlyAppointmentCount>();
            
            // Generate data for each of the last 6 months
            for (int i = 0; i < 6; i++)
            {
                var targetDate = new DateOnly(today.Year, today.Month, 1).AddMonths(-5 + i);
                var monthStart = targetDate;
                var monthEnd = new DateOnly(targetDate.Year, targetDate.Month, DateTime.DaysInMonth(targetDate.Year, targetDate.Month));
                
                var monthlyCount = appointments.Count(a => a.AppointmentDate >= monthStart && a.AppointmentDate <= monthEnd);
                
                monthlyData.Add(new MonthlyAppointmentCount
                {
                    Year = targetDate.Year,
                    Month = targetDate.Month,
                    Count = monthlyCount
                });
            }
            
            // Get today's appointments count
            var todayCount = appointments.Count(a => a.AppointmentDate == today);
            
            response.Response = new AppointmentSelectIn6MonthsEntity
            {
                MonthlyData = monthlyData,
                TodayCount = todayCount,
                TodayDate = today
            };
            
            response.Success = true;
            response.SetMessage(MessageId.I00001);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.SetMessage(MessageId.E00000, $"Error retrieving appointment statistics: {ex.Message}");
        }
        
        return response;
    }
}