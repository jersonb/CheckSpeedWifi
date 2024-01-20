using CheckSpeedWifi.Data;
using CheckSpeedWifi.WorkerService;
using Coravel;
using Serilog;

Console.WriteLine("CheckSpeedWifi starting...");
var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

services.AddSerilog(lc =>
{
    lc.MinimumLevel.Debug()
      .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
      .Enrich.FromLogContext()
      .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}");
});

services.AddSingleton<IDataService, DataService>();

services.AddTransient<Worker>();

services.AddScheduler();

var host = builder.Build();

host.Services.UseScheduler(scheduler =>
{
    scheduler
    .Schedule<Worker>()
    .EveryFifteenMinutes()
    .RunOnceAtStart()
    .PreventOverlapping(nameof(Worker));
});

var dataservice = host.Services.GetRequiredService<IDataService>();

await dataservice.CreateDatabase();

Console.WriteLine("CheckSpeedWifi started!");
host.Run();