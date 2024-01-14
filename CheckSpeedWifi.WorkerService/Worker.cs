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
                //var command = "speedtest-cli --json";
                //var response = await command.ExecuteCommandBashAsync();
                //var result = JsonSerializer.Deserialize<SpeedTestCliResult>(response)!;
                //Console.WriteLine(result);
                _logger.LogInformation("Executed: {@AtTime}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem: {@AtTime}", DateTimeOffset.Now);
            }
        }
    }
}