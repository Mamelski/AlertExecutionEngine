using AlertExecutionEngine.AlertsService.Http.ServiceRegistration;
using AlertExecutionEngine.Core.ServiceRegistration;
using AlertExecutionEngine.Domain.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Read configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Build di container
var serviceProvider = new ServiceCollection()
    .AddHttpAlertsService(configuration)
    .AddDomainServices(configuration)
    .BuildServiceProvider();

var engine = serviceProvider.GetRequiredService<IMonitoringEngine>();

// Start monitoring engine
var cancellationTokenSource = new CancellationTokenSource();
var runningEngineTask = engine.Start(cancellationTokenSource.Token);

// Wait will someone writes "q" to quit
Console.WriteLine("Write \'q\' to quit");
while(Console.ReadLine() != "q")
{
    // Do nothing
}

cancellationTokenSource.Cancel();

try
{
    await runningEngineTask;
}
catch(TaskCanceledException)
{
    // I could think of handling this cancellation inside engine and probably should
    // in real project, but for now I assume, that when cancellation is requested we are just stopping everything
    // without any actions, without saving progress, closing connections, logging, etc.
}

Console.WriteLine("Finished!");