using TCPIPListenerWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<TCPIPListenerBGS>();
    })
    .Build();

await host.RunAsync();
