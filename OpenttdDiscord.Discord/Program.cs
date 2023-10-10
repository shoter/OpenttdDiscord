// See https://aka.ms/new-console-template for more information

using System.Runtime.Loader;
using Akka.Configuration;
using Microsoft.Extensions.Hosting;
using OpenttdDiscord;
using OpenttdDiscord.Discord;

var host = ApplicationBuilder
    .CreateHostBuilder()
    .Build();

var cts = new CancellationTokenSource();
AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

await host.RunAsync(cts.Token);
