using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace COD.FlakeDN.Client
{
    public class FlakeDNClient : IFlakeIdGenerator
    {
        private string server;
        private int port;
        private int? authToken;
        private TcpClient client;
        private object LockObject = new object();


        public FlakeDNClient(string serverHostname, int portNumber, int? authToken)
        {
            this.server = serverHostname;
            this.port = portNumber;
            this.authToken = authToken;
            this.client = new TcpClient();


        }

        public Int64 NewId()
        {
            return NewIds().First();
        }

        public IEnumerable<Int64> NewIds(int numberOfIds = 1)
        {
            lock (LockObject)
            {
                if (!client.Connected)
                {
                    client.Connect(server, port);
                }

                var stream = client.GetStream();

                var reader = new BinaryReader(stream);
                var writer = new BinaryWriter(stream);


                if (authToken.HasValue)
                {
                    writer.Write(authToken.Value);
                }
                writer.Write(0x01);


                writer.Flush();

                List<Int64> ids = new List<long>();

                for (int x = 0; x < numberOfIds; x++)
                    ids.Add(reader.ReadInt64());

                //client.Close();

                return ids;
            }

        }




    }
}
