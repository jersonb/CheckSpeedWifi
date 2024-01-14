using System.Text.Json;
using CheckSpeedWifi.Domain;
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
                    var command = "speedtest-cli --json";
                    var response = await command.ExecuteCommandBashAsync(stoppingToken);
                    var result = JsonSerializer.Deserialize<SpeedTestCliResult>(response)!;

                    _logger.LogInformation("Result: {@result}", result.ToString());
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}