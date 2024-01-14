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

                _ = response ?? throw new InvalidDataException("No data, internet is Ok?");

                var result = JsonSerializer.Deserialize<SpeedTestCliResult>(response);

                _ = result ?? throw new InvalidCastException("Error on deserialization");

                _logger.LogInformation("Download: {@Download} Upload: {@Upload} Ping: {@Ping}", result.Download, result.Upload, result.Ping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem!");
            }
        }
    }
}