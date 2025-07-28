using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;
using Marten;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.API.Services;

public class ResetCounselorScheduleStatusService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ResetCounselorScheduleStatusService> _logger;
    private DateTime _lastRunDate = DateTime.MinValue;

    public ResetCounselorScheduleStatusService(IServiceProvider serviceProvider,
        ILogger<ResetCounselorScheduleStatusService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            if (now.DayOfWeek == DayOfWeek.Sunday && now.Hour >= 18 && _lastRunDate.Date != now.Date)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        // Update in EF Core
                        var dbContext = scope.ServiceProvider
                            .GetRequiredService<
                                AppointmentService.Infrastructure.Data.Contexts.AppointmentServiceContext>();
                        var details = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                            .ToListAsync(dbContext.Set<CounselorScheduleDetail>(), stoppingToken);
                        foreach (var detail in details)
                        {
                            detail.Status = 1;
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);

                        // Update in Marten
                        var documentSession = scope.ServiceProvider.GetRequiredService<IDocumentSession>();
                        var martenDetails = await documentSession.Query<CounselorScheduleDetailCollection>()
                            .ToListAsync(cancellationToken: stoppingToken);
                        foreach (var detail in martenDetails)
                        {
                            detail.StatusId = 1;
                            documentSession.Store(detail);
                        }

                        await documentSession.SaveChangesAsync(stoppingToken);
                    }

                    _logger.LogInformation("CounselorScheduleDetail status reset to 1 at {Time}", now);
                    _lastRunDate = now;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error resetting CounselorScheduleDetail status");
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}