using COD.FlakeDN.Generator;
using COD.FlakeDN.Service.Configuration;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace COD.FlakeDN.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Get Configuration
                var configuration = GetConfigurationSettings();
                var serviceConfiguration = configuration.GetSection("serviceConfiguration").Get<ServiceConfiguration>();

                var app = new CommandLineApplication();
                app.Name = "flakedn";
                app.HelpOption("-?|-h|--help");
                var nodeOption = app.Option("-n|--node", "the node id for this generator", CommandOptionType.SingleValue);
                var portOption = app.Option("-p|--port", "the base network port number", CommandOptionType.SingleValue);


                app.OnExecute(() =>
                {

                    int.TryParse(nodeOption.HasValue() ? nodeOption.Value() : serviceConfiguration.NodeId, out int nodeId);
                    int.TryParse(portOption.HasValue() ? portOption.Value() : serviceConfiguration.PortNumber, out int portNumber);

                    if (nodeId == default)
                        nodeId = 1;

                    if (portNumber == default)
                        portNumber = 0x1D1D;

                    var genParams = new GeneratorParameters
                    {
                        NodeId = nodeId
                    };

                    var generator = new FlakeIdGenerator(genParams);

                    CancellationTokenSource cancelTokenSource = new CancellationTokenSource();


                    Console.CancelKeyPress += (sender, e) => cancelTokenSource.Cancel();

                    RunNetworkService(portNumber, generator, cancelTokenSource.Token);

                    Console.WriteLine("Running");

                    while (!cancelTokenSource.IsCancellationRequested)
                        Console.ReadLine();

                    return 0;
                });

                app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }

        static List<Thread> threads = new List<Thread>();

        private static void RunNetworkService(int basePortNumber, FlakeIdGenerator generator, CancellationToken cancelToken)
        {
            var thread = new Thread(() =>
            {
                var service = new FlakeService(System.Net.IPAddress.Any, basePortNumber, generator);
                service.Run(cancelToken);
            });
            threads.Add(thread);
            thread.Start();
        }

        private static IConfiguration GetConfigurationSettings()
        {
            var configBuilder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return configBuilder.Build();
        }
    }
}
