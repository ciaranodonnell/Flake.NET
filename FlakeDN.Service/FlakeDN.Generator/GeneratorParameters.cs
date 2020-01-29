using System;
using System.Collections.Generic;
using System.Text;

namespace COD.FlakeDN.Generator
{
    public class GeneratorParameters
    {

        ITimeSource _TimeSource;

        public int NodeId { get; set; }



        public int NodeBits { get; set; } = 10;
        public int SequenceBits { get; set; } = 12;


        public bool SpinWhenSequenceExhausted { get; set; } = true;


        public ITimeSource TimeSource
        {
            get => _TimeSource ?? new SystemClockTimeSource(new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            set => _TimeSource = value;
        }

        public long GetSequenceNumber(long Id)
        {
            var sequenceMask = -1L ^ (-1L << SequenceBits);
            var sequence = Id & sequenceMask;
            return sequence;

        }


        public string GetIdParts(long Id)
        {

            var timestampLeftShift = this.SequenceBits + this.NodeBits;
            var sequenceBits = this.SequenceBits;
            var sequenceMask = -1L ^ (-1L << this.SequenceBits);
            var nodeMask = (-1L ^ (-1L << timestampLeftShift)) ^ sequenceMask;
            var time = Id >> timestampLeftShift;
            var sequence = Id & sequenceMask;
            var node = (Id & nodeMask) >> sequenceBits;

            return $"Timestamp={time} (which is {TimeSpan.FromTicks(time * TimeSource.ClockIntervalInTicks).ToString()}), Node = {node}, sequence = {sequence}";
        }
    }
}
