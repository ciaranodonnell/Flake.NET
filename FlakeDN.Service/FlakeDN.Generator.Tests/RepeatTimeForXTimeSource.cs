using System;
using System.Collections.Generic;
using System.Text;

namespace COD.FlakeDN.Generator.Tests
{
    class RepeatTimeForXTimeSource : ITimeSource
    {
        private int repeats;
        private int calls;

        public RepeatTimeForXTimeSource(int numberToRepeat)
        {
            this.repeats = Math.Max(numberToRepeat, 1);
            this.calls = 0;
        }
        public long GetCurrentTime()
        {
            calls++;
            return 1 + (calls / repeats);
        }

        public int Calls { get => calls; }
    }
}
