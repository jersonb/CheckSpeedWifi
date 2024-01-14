using CheckSpeedWifi.WorkerService;
using Coravel;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;

//services.AddHostedService<Worker>();

services.AddTransient<Worker>();

services.AddScheduler();

var host = builder.Build();

host.Services.UseScheduler(scheduler => 
{
    scheduler
    .Schedule<Worker>()
    .EveryFiveSeconds();
    //.EveryTenMinutes();
});
host.Run();