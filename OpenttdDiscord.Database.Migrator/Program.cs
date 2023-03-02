﻿// See https://aka.ms/new-console-template for more informationI

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using OpenttdDiscord.Database;
using System.Reflection;

Console.WriteLine($"args : {string.Join(", ", args)}");

if (args.Count() == 0 || !File.Exists(args[0]))
{
    Console.WriteLine("SQL migration file does not exist!");
    return;
}
ConfigurationBuilder configuration = new();
configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
configuration.AddEnvironmentVariables();
var config = configuration.Build();

string connectionString = config["Database:ConnectionString"]!;
var optionsBuilder = new DbContextOptionsBuilder<OttdContext>();
optionsBuilder.UseNpgsql(connectionString, x =>
{
    x.MigrationsHistoryTable("__MigrationHistory");
});

OttdContext context = new(optionsBuilder.Options);
string sql = File.ReadAllText(args[0]);

Console.WriteLine("Applying script");

for (int i = 0; i < 10; ++i)
{
    try
    {
        await context.Database.ExecuteSqlRawAsync(sql);
        Console.WriteLine("Connected and applied migrations :)");
        break;
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
        await Task.Delay(TimeSpan.FromSeconds(2));
    }
}

Console.WriteLine("Done");
