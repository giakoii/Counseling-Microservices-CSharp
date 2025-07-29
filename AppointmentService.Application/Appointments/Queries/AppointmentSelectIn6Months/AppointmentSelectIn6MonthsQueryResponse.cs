using Common;

namespace AppointmentService.Application.Appointments.Queries.AppointmentSelectIn6Months;

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
