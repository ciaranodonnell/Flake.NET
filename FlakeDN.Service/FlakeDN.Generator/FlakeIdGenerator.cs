using COD.FlakeDN.Client;
using System;

namespace COD.FlakeDN.Generator
{
    public class FlakeIdGenerator
    {
        private long nodeId;
        private int timestampLeftShift;
        private int sequenceBits;
        private long sequenceMask;
        private long nodeMask;
        private bool spinOnSequenceExhaustion;
        private long lastTime=-1;
        private long currentSequence;

        private object LockObject = new object();
        private ITimeSource clock;


        public FlakeIdGenerator(GeneratorParameters parameters)
        {
            Initialize(parameters);
        }

        void Initialize(GeneratorParameters parameters)
        {
            
            this.nodeId = Convert.ToInt64(parameters.NodeId) << parameters.SequenceBits;
            this.timestampLeftShift = parameters.SequenceBits + parameters.NodeBits;
            this.sequenceBits = parameters.SequenceBits;
            this.sequenceMask = -1L ^ (-1L << parameters.SequenceBits);
            this.nodeMask = (-1L ^ (-1L << timestampLeftShift)) ^ sequenceMask;
            this.spinOnSequenceExhaustion = parameters.SpinWhenSequenceExhausted;

            clock = parameters.TimeSource ?? new SystemClockTimeSource(new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

        }


        public Int64 NewId()
        {
            lock (LockObject)
            {
                var time = clock.GetCurrentTime();

                if (time < lastTime)
                {
                    return (Int64)FlakeDNErrors.TimeMovingBackwards;
                }


                if (lastTime == time)
                {
                    var nextSequence = currentSequence+1;

                    if ((nextSequence & sequenceMask) < currentSequence)
                    {
                        if (spinOnSequenceExhaustion)
                        {
                            while ((time = clock.GetCurrentTime()) == lastTime) ;
                            currentSequence = 0;
                            lastTime = time;
                        }
                        else
                        {
                            //check if sequence will loop for the current millisecond number. return error
                            return (long)FlakeDNErrors.SequenceExhausted;
                        }
                    }
                    else
                    {
                        ++currentSequence;
                    }
                }
                else
                {
                    currentSequence = 0;
                    lastTime = time;
                }

                long id = (time << timestampLeftShift) | nodeId | currentSequence;

                return id;
            }
        }


        public string GetIdParts(long Id)
        {
            var milliseconds = Id >> timestampLeftShift;
            var sequence = Id & sequenceMask;
            var node = (Id & nodeMask) >> sequenceBits;

            return $"MS={milliseconds} (which is {TimeSpan.FromMilliseconds(milliseconds).ToString()}), Node = {node}, sequence = {sequence}";
        }
    }
}
