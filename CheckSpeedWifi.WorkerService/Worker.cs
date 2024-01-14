using System.Text.Json;
using CheckSpeedWifi.Domain;
using Coravel.Invocable;
using ExecuteTerminal;

namespace CheckSpeedWifi.WorkerService
{
    public class Worker(ILogger<Worker> logger) : IInvocable
    {
        private readonly ILogger<Worker> _logger = logger;

        public async Task Invoke()
        {
            try
            {
                var command = "speedtest-cli --json";
                var response = await command.ExecuteCommandBashAsync();
                var result = JsonSerializer.Deserialize<SpeedTestCliResult>(response)!;
                _logger.LogInformation("Executed: {@AtTime} Download: {@Download} Upload: {@Upload}", DateTimeOffset.Now, result.Download, result.Upload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem: {@AtTime}", DateTimeOffset.Now);
            }
        }
    }
}