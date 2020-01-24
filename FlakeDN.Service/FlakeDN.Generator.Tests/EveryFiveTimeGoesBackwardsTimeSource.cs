using System;
using System.Collections.Generic;
using System.Text;

namespace COD.FlakeDN.Generator.Tests
{
    class EveryFiveTimeGoesBackwardsTimeSource : ITimeSource
    {
        private int calls;

        public EveryFiveTimeGoesBackwardsTimeSource()
        {
            this.calls = 0;
        }

        public long GetCurrentTime()
        {
            calls++;
            if (calls % 5 == 0) calls = calls - 2;
            return calls;
        }
    }
}
