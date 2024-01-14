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
                await CheckSpeedWifi(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private async Task CheckSpeedWifi(CancellationToken stoppingToken)
        {
            try
            {
                var command = "speedtest-cli --json";
                var response = await command.ExecuteCommandBashAsync(stoppingToken);
                var result = JsonSerializer.Deserialize<SpeedTestCliResult>(response)!;
                Console.WriteLine(result);
                _logger.LogInformation("Executed: {@AtTime}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem: {@AtTime}", DateTimeOffset.Now);
            }
        }
    }
}