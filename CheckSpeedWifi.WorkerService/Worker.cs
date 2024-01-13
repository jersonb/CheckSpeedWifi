using ExecuteTerminal;

namespace CheckSpeedWifi.WorkerService
{
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    var command = "date";
                    var result = await command.ExecuteCommandBashAsync(stoppingToken);
                    _logger.LogInformation("Worker running at: {time}", result);
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}