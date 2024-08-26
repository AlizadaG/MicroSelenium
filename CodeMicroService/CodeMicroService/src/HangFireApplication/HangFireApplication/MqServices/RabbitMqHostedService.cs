using MassTransit;

namespace HangFireApplication.MqServices;

public class RabbitMqHostedService : BackgroundService
{
    private readonly IBusControl _busControl;
    private readonly ILogger<RabbitMqHostedService> _logger;
    public RabbitMqHostedService(IBusControl busControl, ILogger<RabbitMqHostedService> logger)
    {
        _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMqHostedService is starting.");

        try
        {
            await _busControl.StartAsync(stoppingToken);
            _logger.LogInformation("RabbitMq Hosted Service started successfully.");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception)
        {
            _logger.LogError("RabbitMq Hosted Service failed to start.");
            throw;
        }
        finally
        {
            await _busControl.StopAsync(stoppingToken);
            _logger.LogInformation("RabbitMq Hosted Service stopped.");
        }
    }
}
