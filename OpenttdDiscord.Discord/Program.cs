// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using OpenttdDiscord;
using System.Runtime.Loader;

var host = ApplicationBuilder
    .CreateHostBuilder()
    .Build();

var cts = new CancellationTokenSource();
AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

await host.RunAsync(cts.Token);
