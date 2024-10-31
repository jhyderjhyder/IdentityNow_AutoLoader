

using ISC_AutoLoader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



IHost host = Host.CreateDefaultBuilder(args).UseWindowsService(options =>
{
    options.ServiceName = "ISC AutoLoader";
})
    .ConfigureServices(services =>
{
    services.AddHostedService<FileScanner>();
})
.Build();

await host.RunAsync();  