using System;
using System.Collections.Generic;
using System.Text;

namespace COD.FlakeDN.Generator
{
    public class GeneratorParameters
    {


        public int NodeId { get; set; }



        public int NodeBits { get; set; } = 10;
        public int SequenceBits { get; set; } = 12;
        

        public bool SpinWhenSequenceExhausted { get; set; } = true;
        public ITimeSource TimeSource { get; set; }

        public long GetSequenceNumber(long Id)
        {
            var sequenceMask = -1L ^ (-1L << SequenceBits);
            var sequence = Id & sequenceMask;
            return sequence;

        }
    }
}
