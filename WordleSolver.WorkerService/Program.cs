using WordleSolver.Domain;
using WordleSolver.WorkerService;
using Serilog;
using Serilog.Sinks.FastConsole;
using System.Text;
using Microsoft.Net.Http.Headers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        Console.OutputEncoding = Encoding.Unicode;

        services.AddHttpClient("Twitter")
       .ConfigureHttpClient(httpClient =>
       {
           httpClient.BaseAddress = new Uri("https://api.twitter.com");
           httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, "Bearer AAAAAAAAAAAAAAAAAAAAAOa6OgEAAAAAVJH%2FoLHMrxJJjBfbu6tPEZX%2F%2B%2Bc%3DuP95r6ncH1iH0CavU5RK0jFrwZ1C5e9yHDFoUJ1G4D7aM4ph1A");
       });

        services.AddTransient<ITwitterService, TwitterService>();
        services.AddTransient<IWordleWordSolver, WordleWordSolver>();
        services.AddHostedService<Worker>();
    })
    .UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console())
    .Build();

await host.RunAsync();
