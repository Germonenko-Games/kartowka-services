using Kartowka.Core;
using Microsoft.EntityFrameworkCore;

namespace Kartowka.Api.HostedServices;

public class ApplyMigrationsHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<ApplyMigrationsHostedService> _logger;

    public ApplyMigrationsHostedService(
        IServiceProvider serviceProvider,
        ILogger<ApplyMigrationsHostedService> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope();

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<CoreContext>();

            _logger.LogInformation("Started applying migrations");
            await context.Database.MigrateAsync(stoppingToken);
            _logger.LogInformation("Sucusefully applied migrations");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to apply migrations");
        }
        finally
        {
            scope.Dispose();
        }
    }
}
