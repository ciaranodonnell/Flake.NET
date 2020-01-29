using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace COD.FlakeDN
{
    public interface IFlakeIdGenerator
    {
        Int64 NewId();

        IEnumerable<Int64> NewIds(int numberOfIds);

    }
}
