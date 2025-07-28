using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.Snapshorts;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.Appointments.Queries;

public class AppointmentSelectIn6MonthsQuery : IQuery<AppointmentSelectIn6MonthsQueryResponse>
{
}

/// <summary>
/// AppointmentSelectIn6MonthsQueryHandler - Handles the retrieval of appointment counts for the last 6 months and today.
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

public class AppointmentSelectIn6MonthsQueryResponse : AbstractResponse<AppointmentSelectIn6MonthsEntity>
{
    public override AppointmentSelectIn6MonthsEntity Response { get; set; } = null!;
}

public class AppointmentSelectIn6MonthsEntity
{
    public List<MonthlyAppointmentCount> MonthlyData { get; set; } = new();
    public int TodayCount { get; set; }
    public DateOnly TodayDate { get; set; }
}

public class MonthlyAppointmentCount
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Count { get; set; }
}
