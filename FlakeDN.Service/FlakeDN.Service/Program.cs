using COD.FlakeDN.Generator;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace COD.FlakeDN.Service
{
    class Program
    {
        static void Main(string[] args)
        {

            var app = new CommandLineApplication();
            app.Name = "flakedn";
            app.HelpOption("-?|-h|--help");
            var nodeOption = app.Option("-n|--node", "the node id for this generator", CommandOptionType.SingleValue);
            var portOption = app.Option("-p|--port", "the base network port number", CommandOptionType.SingleValue);


            app.OnExecute(() =>
            {
                int basePortNumber = 0x1D1D;
                int nodeId = 1;

                if (nodeOption.HasValue())
                {
                    nodeId = int.Parse(nodeOption.Value());
                }

                if (portOption.HasValue())
                {
                    nodeId = int.Parse(portOption.Value());
                }
                                
                var genParams = new GeneratorParameters
                {
                    NodeId = nodeId
                };

                var generator = new FlakeIdGenerator(genParams);

                CancellationTokenSource cancelTokenSource = new CancellationTokenSource();


                Console.CancelKeyPress += (sender, e) => cancelTokenSource.Cancel();

                RunNetworkService(basePortNumber, generator, cancelTokenSource.Token);

                Console.WriteLine("Running");

                while (!cancelTokenSource.IsCancellationRequested)
                    Console.ReadLine();

                return 0;
            });

            app.Execute(args);
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
    }
}
