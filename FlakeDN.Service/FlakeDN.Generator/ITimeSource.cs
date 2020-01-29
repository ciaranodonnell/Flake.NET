using System;
using System.Collections.Generic;
using System.Text;

namespace COD.FlakeDN.Generator
{
    public interface ITimeSource
    {
        long GetCurrentTime();

        int ClockIntervalInTicks { get; }
    }
}
