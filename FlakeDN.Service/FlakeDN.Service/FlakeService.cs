using COD.FlakeDN.Generator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace COD.FlakeDN.Service
{
    public class FlakeService
    {
        private IPAddress ipAddress;
        private int portNumber;
        private int? authTokenNumber;
        private FlakeIdGenerator idGenerator;
        private IPAddress any;
        private int v;
        private FlakeIdGenerator generator;

        public FlakeService(IPAddress ipAddress, int portNumber, FlakeIdGenerator generator, int? authTokenNumber=null)
        {
            this.ipAddress = ipAddress;
            this.portNumber = portNumber;
            this.authTokenNumber = authTokenNumber;


            this.idGenerator = generator;

        }
             

        public void Run(CancellationToken cancelToken)
        {

            TcpListener listener = new TcpListener(ipAddress, portNumber);
            listener.Start();
            IsListening = true;

            while (IsListening && !cancelToken.IsCancellationRequested)
            {
                var client = listener.AcceptTcpClient();
                var stream = client.GetStream();
                var reader = new BinaryReader(stream);
                var writer = new BinaryWriter(stream);
                if (authTokenNumber.HasValue)
                {
                    var recAuth = reader.ReadInt32();
                    if (recAuth != authTokenNumber.Value)
                    {
                        writer.Write(-1L);
                        writer.Flush();
                        client.Close();
                    }

                    int numIds = (int)reader.ReadByte();

                    for(int x = 0; x < numIds; x++)
                    {
                        writer.Write(idGenerator.NewId());
                    }
                    writer.Flush();
                    client.Close();
                    
                }
            }
        }


        private bool IsListening { get; set; }
    }
}
