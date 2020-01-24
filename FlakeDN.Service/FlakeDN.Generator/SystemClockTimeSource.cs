using System;
using System.Collections.Generic;
using System.Text;

namespace COD.FlakeDN.Generator
{
    public class SystemClockTimeSource : ITimeSource
    {
        private DateTime epoch;

        public SystemClockTimeSource(DateTime epoch)
        {
            this.epoch = epoch;
        }


        public long GetCurrentTime()
        {
            long ms = Convert.ToInt64(DateTime.UtcNow.Subtract(epoch).TotalMilliseconds);
            return ms;
        }
    }
}
