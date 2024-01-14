using CheckSpeedWifi.WorkerService;
using Coravel;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;

services.AddTransient<Worker>();

services.AddScheduler();

var host = builder.Build();

host.Services.UseScheduler(scheduler =>
{
    scheduler
    .Schedule<Worker>()
    .EveryTenMinutes()
    .PreventOverlapping(nameof(Worker));
});

host.Run();